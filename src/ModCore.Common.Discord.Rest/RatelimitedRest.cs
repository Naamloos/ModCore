using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Rest
{
    public class RateLimitedRest
    {
        const short API_VERSION = 10;

        private HttpClient httpClient;
        private DiscordRestConfiguration configuration;
        private JsonSerializerOptions jsonSerializerOptions;

        private ConcurrentDictionary<string, RateLimitBucket> buckets;

        public RateLimitedRest(DiscordRestConfiguration config, JsonSerializerOptions jsonSerializerOptions)
        {
            this.jsonSerializerOptions = jsonSerializerOptions;
            buckets = new ConcurrentDictionary<string, RateLimitBucket>();
            configuration = config;

            httpClient = new HttpClient()
            {
                BaseAddress = new Uri($"https://discord.com/api/v{API_VERSION}")
            };

            httpClient.DefaultRequestHeaders.Add("Authorization", $"{configuration.TokenType} {configuration.Token}");
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ModCore3 (https://github.com/Naamloos/ModCore)");
        }

        public async Task<HttpResponseMessage> RequestAsync(HttpMethod method, string route, string url, object? body = null)
        {
            RateLimitBucket bucket;

            if(!buckets.TryGetValue(route, out bucket))
            {
                bucket = new RateLimitBucket();
                buckets.TryAdd(route, bucket);
            }

            await bucket.WaitAsync();

            var request = new HttpRequestMessage(method, url);
            if(body != null)
            {
                request.Content = JsonContent.Create(body, options: jsonSerializerOptions);
            }

            var response = await httpClient.SendAsync(request);

            var remaining = response.Headers.GetValues("X-RateLimit-Remaining");
            var reset_after = response.Headers.GetValues("X-RateLimit-Reset-After");

            await bucket.SignalDoneAsync(int.Parse(remaining.First()), float.Parse(reset_after.First()));

            return response;
        }
    }
}
