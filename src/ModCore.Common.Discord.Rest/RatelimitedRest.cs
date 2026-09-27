using System.Collections.Concurrent;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ModCore.Common.Discord.Rest
{
    public class RateLimitedRest
    {
        private readonly HttpClient httpClient;
        private readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly bool proxyEnabled;
        private readonly AuthenticationHeaderValue? authorization;
        private readonly SemaphoreSlim requestLock = new(1, 1);
        private readonly Dictionary<string, string> routeBuckets = new();
        private readonly Dictionary<string, DateTimeOffset> resetTimes = new();
        private DateTimeOffset globalReset;

        public RateLimitedRest(
            DiscordRestConfiguration config,
            JsonSerializerOptions jsonSerializerOptions
        )
        {
            this.jsonSerializerOptions = jsonSerializerOptions;
            proxyEnabled = !string.IsNullOrWhiteSpace(config.RestProxy);
            var baseAddress = proxyEnabled ? config.RestProxy : "https://discord.com";
            httpClient =
                config.HttpMessageHandler == null
                    ? new HttpClient()
                    : new HttpClient(config.HttpMessageHandler, disposeHandler: false);
            httpClient.BaseAddress = new Uri($"{baseAddress.TrimEnd('/')}/api/v10/");
            httpClient.Timeout = TimeSpan.FromSeconds(60);
            if (!string.IsNullOrEmpty(config.Token))
            {
                authorization = new(config.AuthType, config.Token);
            }
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "DiscordBot (https://github.com/Naamloos/ModCore, 3.0)"
            );
        }

        public ValueTask<HttpResponseMessage> RequestAsync(
            HttpMethod method,
            string route,
            string url,
            object? body = null,
            bool asForm = false
        ) => RequestAsync(method, route, url, body, asForm, null, CancellationToken.None);

        public async ValueTask<HttpResponseMessage> RequestAsync(
            HttpMethod method,
            string route,
            string url,
            object? body,
            bool asForm,
            string? auditLogReason,
            CancellationToken cancellationToken
        )
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var absolute))
            {
                if (
                    absolute.Scheme != "https"
                    || absolute.Host != "discord.com"
                    || !absolute.AbsolutePath.StartsWith("/api/", StringComparison.Ordinal)
                )
                    throw new ArgumentException("Only Discord API URLs are accepted.", nameof(url));
                url = absolute.PathAndQuery[5..];
                if (url.StartsWith("v10/", StringComparison.Ordinal))
                    url = url[4..];
            }

            // The proxy owns all rate limiting and retries. Do not acquire local locks or inspect rate-limit headers here.
            if (proxyEnabled)
                return await SendAsync(
                    method,
                    url,
                    body,
                    asForm,
                    auditLogReason,
                    cancellationToken
                );

            // Serialize direct requests so an exhausted bucket cannot be raced by concurrent requests.
            await requestLock.WaitAsync(cancellationToken);
            try
            {
                var key = $"{method}:{route}";
                var major = MajorResource(url);
                for (var attempt = 0; ; attempt++)
                {
                    var bucketKey = routeBuckets.GetValueOrDefault(key, key) + ":" + major;
                    var reset = resetTimes.GetValueOrDefault(bucketKey);
                    if (globalReset > reset)
                        reset = globalReset;
                    var delay = reset - DateTimeOffset.UtcNow;
                    if (delay > TimeSpan.Zero)
                        await Task.Delay(delay, cancellationToken);

                    var response = await SendAsync(
                        method,
                        url,
                        body,
                        asForm,
                        auditLogReason,
                        cancellationToken
                    );
                    if (response.Headers.TryGetValues("X-RateLimit-Bucket", out var hashes))
                    {
                        routeBuckets[key] = hashes.First();
                        bucketKey = routeBuckets[key] + ":" + major;
                    }
                    if (
                        HeaderNumber(response, "X-RateLimit-Remaining") is <= 0
                        && HeaderNumber(response, "X-RateLimit-Reset-After") is double seconds
                    )
                        resetTimes[bucketKey] = DateTimeOffset.UtcNow.AddSeconds(
                            Math.Max(0, seconds)
                        );

                    if (response.StatusCode != HttpStatusCode.TooManyRequests)
                        return response;
                    double? retryAfter = HeaderNumber(response, "Retry-After");
                    var global = response.Headers.Contains("X-RateLimit-Global");
                    try
                    {
                        using var json = JsonDocument.Parse(
                            await response.Content.ReadAsStringAsync(cancellationToken)
                        );
                        if (
                            json.RootElement.TryGetProperty("retry_after", out var retry)
                            && retry.TryGetDouble(out var number)
                        )
                            retryAfter = number;
                        if (json.RootElement.TryGetProperty("global", out var flag))
                            global |= flag.ValueKind == JsonValueKind.True;
                    }
                    catch (JsonException) { }
                    if (retryAfter is not double wait || !double.IsFinite(wait) || wait < 0)
                        return response;
                    var until = DateTimeOffset.UtcNow.AddSeconds(wait);
                    if (global)
                        globalReset = until;
                    else
                        resetTimes[bucketKey] = until;
                    if (attempt >= 4)
                        return response;
                    response.Dispose();
                }
            }
            finally
            {
                requestLock.Release();
            }
        }

        private async Task<HttpResponseMessage> SendAsync(
            HttpMethod method,
            string url,
            object? body,
            bool asForm,
            string? reason,
            CancellationToken cancellationToken
        )
        {
            using var request = new HttpRequestMessage(method, url);
            var parts = url.Split('?', 2)[0].Trim('/').Split('/');
            var tokenAuthenticated =
                parts[0] == "interactions"
                || (parts[0] == "webhooks" && parts.Length >= 3)
                || url.StartsWith("oauth2/token", StringComparison.Ordinal);
            if (!tokenAuthenticated)
                request.Headers.Authorization = authorization;
            if (reason != null)
                request.Headers.Add("X-Audit-Log-Reason", Uri.EscapeDataString(reason));
            if (body is MultipartRequest multipart)
            {
                var content = new MultipartFormDataContent();
                if (multipart.Payload != null)
                    content.Add(
                        new StringContent(
                            JsonSerializer.Serialize(multipart.Payload, jsonSerializerOptions),
                            Encoding.UTF8,
                            "application/json"
                        ),
                        "payload_json"
                    );
                foreach (var field in multipart.Fields)
                    content.Add(new StringContent(field.Value), field.Key);
                for (var i = 0; i < multipart.Files.Count; i++)
                {
                    var file = multipart.Files[i];
                    var bytes = new ByteArrayContent(file.Data);
                    if (file.ContentType != null)
                        bytes.Headers.ContentType = new(file.ContentType);
                    content.Add(bytes, file.FieldName ?? $"files[{i}]", file.FileName);
                }
                request.Content = content;
            }
            else if (body != null)
            {
                if (asForm)
                {
                    if (body is not IEnumerable<KeyValuePair<string, string>> fields)
                        throw new ArgumentException(
                            "Form bodies must contain string key/value pairs.",
                            nameof(body)
                        );
                    request.Content = new FormUrlEncodedContent(fields);
                }
                else
                    request.Content = JsonContent.Create(body, options: jsonSerializerOptions);
            }
            return await httpClient.SendAsync(request, cancellationToken);
        }

        private static double? HeaderNumber(HttpResponseMessage response, string header) =>
            response.Headers.TryGetValues(header, out var values)
            && double.TryParse(
                values.FirstOrDefault(),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out var value
            )
            && double.IsFinite(value)
                ? value
                : null;

        private static string MajorResource(string url)
        {
            var parts = url.Split('?', 2)[0].Trim('/').Split('/');
            if (parts.Length < 2)
                return "";
            return parts[0] switch
            {
                "channels" or "guilds" => parts[1],
                "webhooks" => string.Join('/', parts.Skip(1).Take(2)),
                _ => "",
            };
        }
    }
}
