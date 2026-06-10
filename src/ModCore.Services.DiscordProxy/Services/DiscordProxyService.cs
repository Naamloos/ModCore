using ModCore.Services.DiscordProxy.Services;
using System.Net.Http.Headers;

namespace ModCore.Services.DiscordProxy.Services
{
    public sealed class DiscordProxyService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DiscordRateLimiter _rateLimiter;
        private readonly ILogger<DiscordProxyService> _logger;

        public DiscordProxyService(
            IHttpClientFactory httpClientFactory,
            DiscordRateLimiter rateLimiter,
            ILogger<DiscordProxyService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _rateLimiter = rateLimiter;
            _logger = logger;
        }

        public async Task ProxyAsync(
            HttpContext context,
            string discordPath,
            CancellationToken cancellationToken)
        {
            var method = new HttpMethod(context.Request.Method);

            if (!IsAllowedMethod(method))
            {
                context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                return;
            }

            _logger.LogInformation(
                "{Method} {Path}",
                method,
                discordPath);

            var bodyBytes = await ReadRequestBodyAsync(
                context,
                cancellationToken);

            var routeKey = DiscordRouteKey.Normalize(
                method.Method,
                discordPath);

            var client = _httpClientFactory.CreateClient("discord");

            var discordResponse = await _rateLimiter.ExecuteAsync(
                routeKey,
                async ct =>
                {
                    using var discordRequest = CreateDiscordRequest(
                        context,
                        method,
                        discordPath,
                        bodyBytes);

                    _logger.LogDebug(
                        "Forwarding Discord request {Method} {Path}",
                        method,
                        discordPath);

                    return await client.SendAsync(
                        discordRequest,
                        HttpCompletionOption.ResponseHeadersRead,
                        ct);
                },
                cancellationToken);

            using (discordResponse)
            {
                await CopyDiscordResponseToClientAsync(
                    discordResponse,
                    context,
                    cancellationToken);
            }
        }

        private static HttpRequestMessage CreateDiscordRequest(
            HttpContext context,
            HttpMethod method,
            string discordPath,
            byte[]? bodyBytes)
        {
            var queryString = context.Request.QueryString.Value ?? string.Empty;

            string qualifiedDiscordPath = discordPath.StartsWith("/") ? discordPath.Substring(1) : discordPath;

            var request = new HttpRequestMessage(
                method,
                qualifiedDiscordPath + queryString);

            if (bodyBytes is { Length: > 0 })
            {
                request.Content = new ByteArrayContent(bodyBytes);
            }

            CopyIncomingHeadersToDiscordRequest(context, request);

            return request;
        }

        private static async Task<byte[]?> ReadRequestBodyAsync(
            HttpContext context,
            CancellationToken cancellationToken)
        {
            if (!RequestMayHaveBody(context.Request))
                return null;

            using var memoryStream = new MemoryStream();

            await context.Request.Body.CopyToAsync(
                memoryStream,
                cancellationToken);

            return memoryStream.ToArray();
        }

        private static bool RequestMayHaveBody(HttpRequest request)
        {
            return request.ContentLength > 0 ||
                   request.Headers.ContainsKey("Transfer-Encoding");
        }

        private static void CopyIncomingHeadersToDiscordRequest(
            HttpContext context,
            HttpRequestMessage discordRequest)
        {
            foreach (var header in context.Request.Headers)
            {
                if (ShouldSkipRequestHeader(header.Key))
                    continue;

                var values = header.Value.ToArray();

                if (discordRequest.Headers.TryAddWithoutValidation(
                        header.Key,
                        values))
                {
                    continue;
                }

                if (discordRequest.Content is not null)
                {
                    discordRequest.Content.Headers.TryAddWithoutValidation(
                        header.Key,
                        values);
                }
            }
        }

        private static bool IsAllowedMethod(HttpMethod method)
        {
            return method == HttpMethod.Get ||
                   method == HttpMethod.Post ||
                   method == HttpMethod.Put ||
                   method == HttpMethod.Patch ||
                   method == HttpMethod.Delete;
        }

        private static bool ShouldSkipRequestHeader(string header)
        {
            return header.Equals("Host", StringComparison.OrdinalIgnoreCase) ||
                   header.Equals("Connection", StringComparison.OrdinalIgnoreCase) ||
                   header.Equals("Content-Length", StringComparison.OrdinalIgnoreCase) ||
                   header.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase) ||
                   header.Equals("Keep-Alive", StringComparison.OrdinalIgnoreCase) ||
                   header.Equals("Proxy-Authenticate", StringComparison.OrdinalIgnoreCase) ||
                   header.Equals("Proxy-Authorization", StringComparison.OrdinalIgnoreCase) ||
                   header.Equals("TE", StringComparison.OrdinalIgnoreCase) ||
                   header.Equals("Trailer", StringComparison.OrdinalIgnoreCase) ||
                   header.Equals("Upgrade", StringComparison.OrdinalIgnoreCase);
        }

        private static async Task CopyDiscordResponseToClientAsync(
            HttpResponseMessage discordResponse,
            HttpContext context,
            CancellationToken cancellationToken)
        {
            context.Response.StatusCode = (int)discordResponse.StatusCode;

            foreach (var header in discordResponse.Headers)
            {
                context.Response.Headers[header.Key] =
                    header.Value.ToArray();
            }

            foreach (var header in discordResponse.Content.Headers)
            {
                context.Response.Headers[header.Key] =
                    header.Value.ToArray();
            }

            context.Response.Headers.Remove("transfer-encoding");

            await discordResponse.Content.CopyToAsync(
                context.Response.Body,
                cancellationToken);
        }
    }
}