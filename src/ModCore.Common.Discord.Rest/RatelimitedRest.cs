using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text.Json;

namespace ModCore.Common.Discord.Rest
{
    public class RateLimitedRest
    {
        const short API_VERSION = 10;
        const string API_BASE = "https://discord.com";

        private HttpClient httpClient;
        private DiscordRestConfiguration configuration;
        private JsonSerializerOptions jsonSerializerOptions;

        private ConcurrentDictionary<string, RateLimitBucket> buckets;

        private bool proxyEnabled = false;
        private string baseAddress = "https://discord.com/";

        public RateLimitedRest(DiscordRestConfiguration config, JsonSerializerOptions jsonSerializerOptions)
        {
            this.jsonSerializerOptions = jsonSerializerOptions;
            buckets = new ConcurrentDictionary<string, RateLimitBucket>();
            configuration = config;

            proxyEnabled = !string.IsNullOrEmpty(config.RestProxy);

            string baseAddress = (proxyEnabled ? config.RestProxy : API_BASE);

            Console.WriteLine($"BaseAddress: {baseAddress}");

            httpClient = new HttpClient()
            {
                BaseAddress = new Uri($"{baseAddress}/api/v{API_VERSION}/"),
                Timeout = TimeSpan.FromSeconds(60) // Proxy might also hit a rate limit.
            };

            // DON'T use this httpclient elsewhere, like sentry or some shit. Just sayin.
            httpClient.DefaultRequestHeaders.Add("Authorization", $"{configuration.AuthType} {configuration.Token}");
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ModCore/3.0 (https://github.com/Naamloos/ModCore)");
        }

        public async ValueTask<HttpResponseMessage> RequestAsync(HttpMethod method, string route, string url, object? body = null, bool asForm = false)
        {
            RateLimitBucket? bucket = null;

            if (!proxyEnabled)
            {
                // TODO (de)serialize from distributed cache
                if (!buckets.TryGetValue(route, out bucket))
                {
                    bucket = new RateLimitBucket();
                    buckets.TryAdd(route, bucket);
                }

                await bucket.WaitAsync();
            }

            var qualifiedUrl = url;
            if(url.Contains("https://discord.com/api"))
            {
                qualifiedUrl = url.Replace("https://discord.com/api", $"{this.baseAddress}/api");
            }

            var request = new HttpRequestMessage(method, url);
            if (body != null)
            {
                if (!asForm)
                {
                    request.Content = JsonContent.Create(body, options: jsonSerializerOptions);
                }
                else
                {
                    IEnumerable<KeyValuePair<string, string>> formValues;

                    if (body is IEnumerable<KeyValuePair<string, string>> keyValuePairs)
                    {
                        formValues = keyValuePairs;
                    }
                    else
                    {
                        formValues = body
                            .GetType()
                            .GetProperties()
                            .Select(prop => new KeyValuePair<string, string>(
                                prop.Name,
                                prop.GetValue(body)?.ToString() ?? string.Empty
                            ));
                    }

                    request.Content = new FormUrlEncodedContent(formValues);
                }
            }

            var response = await httpClient.SendAsync(request);

            if (bucket is not null && response.Headers.Contains("X-Ratelimit-Remaining"))
            {
                var remaining = response.Headers.GetValues("X-RateLimit-Remaining");
                var reset_after = response.Headers.GetValues("X-RateLimit-Reset-After");

                await bucket.SignalDoneAsync(int.Parse(remaining.First()), float.Parse(reset_after.First()));
            }

            return response;
        }
    }
}
