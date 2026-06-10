using System.Collections.Concurrent;
using System.Globalization;
using System.Net;
using System.Text.Json;

namespace ModCore.Services.DiscordProxy.Services
{
    public sealed class DiscordRateLimiter
    {
        private readonly ConcurrentDictionary<string, BucketState> _buckets = new();
        private readonly ConcurrentDictionary<string, string> _routeToBucket = new();

        private readonly SemaphoreSlim _globalLock = new(1, 1);

        private DateTimeOffset _globalResetAt = DateTimeOffset.MinValue;

        public async Task<HttpResponseMessage> ExecuteAsync(
            string routeKey,
            Func<CancellationToken, Task<HttpResponseMessage>> sendAsync,
            CancellationToken cancellationToken)
        {
            // As long as the client doesn't time out, we can keep trying
            while (true)
            {
                var bucketKey = _routeToBucket.TryGetValue(
                    routeKey,
                    out var knownBucket)
                        ? knownBucket
                        : routeKey;

                var bucket = _buckets.GetOrAdd(
                    bucketKey,
                    _ => new BucketState());

                await bucket.Lock.WaitAsync(cancellationToken);

                try
                {
                    await WaitForGlobalLimitAsync(cancellationToken);
                    await WaitForBucketAsync(bucket, cancellationToken);

                    var response = await sendAsync(cancellationToken);

                    await UpdateRateLimitStateAsync(
                        routeKey,
                        bucketKey,
                        bucket,
                        response,
                        cancellationToken);

                    if (response.StatusCode != HttpStatusCode.TooManyRequests)
                    {
                        return response;
                    }

                    var retryAfter = await GetRetryAfterAsync(
                        response,
                        cancellationToken);

                    if (IsGlobalRateLimit(response))
                    {
                        await SetGlobalLimitAsync(retryAfter, cancellationToken);
                    }
                    else
                    {
                        bucket.Remaining = 0;
                        bucket.ResetAt = DateTimeOffset.UtcNow.Add(retryAfter);
                    }

                    response.Dispose();

                    await Task.Delay(retryAfter, cancellationToken);
                }
                finally
                {
                    bucket.Lock.Release();
                }
            }
        }

        private async Task WaitForGlobalLimitAsync(
            CancellationToken cancellationToken)
        {
            await _globalLock.WaitAsync(cancellationToken);

            try
            {
                var delay = _globalResetAt - DateTimeOffset.UtcNow;

                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, cancellationToken);
                }
            }
            finally
            {
                _globalLock.Release();
            }
        }

        private static async Task WaitForBucketAsync(
            BucketState bucket,
            CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow;

            if (bucket.Remaining <= 0 && bucket.ResetAt > now)
            {
                await Task.Delay(bucket.ResetAt - now, cancellationToken);
            }
        }

        private async Task UpdateRateLimitStateAsync(
            string routeKey,
            string oldBucketKey,
            BucketState bucket,
            HttpResponseMessage response,
            CancellationToken cancellationToken)
        {
            if (TryGetHeader(
                    response,
                    "X-RateLimit-Bucket",
                    out var discordBucket))
            {
                _routeToBucket[routeKey] = discordBucket;

                if (discordBucket != oldBucketKey)
                {
                    _buckets.TryAdd(discordBucket, bucket);
                }
            }

            if (TryGetHeader(
                    response,
                    "X-RateLimit-Remaining",
                    out var remainingText) &&
                int.TryParse(
                    remainingText,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out var remaining))
            {
                bucket.Remaining = remaining;
            }

            if (TryGetHeader(
                    response,
                    "X-RateLimit-Reset-After",
                    out var resetAfterText) &&
                double.TryParse(
                    resetAfterText,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var resetAfterSeconds))
            {
                bucket.ResetAt = DateTimeOffset.UtcNow
                    .AddSeconds(resetAfterSeconds);
            }

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                var retryAfter = await GetRetryAfterAsync(
                    response,
                    cancellationToken);

                if (IsGlobalRateLimit(response))
                {
                    await SetGlobalLimitAsync(retryAfter, cancellationToken);
                }
                else
                {
                    bucket.Remaining = 0;
                    bucket.ResetAt = DateTimeOffset.UtcNow.Add(retryAfter);
                }
            }
        }

        private async Task SetGlobalLimitAsync(
            TimeSpan retryAfter,
            CancellationToken cancellationToken)
        {
            await _globalLock.WaitAsync(cancellationToken);

            try
            {
                _globalResetAt = DateTimeOffset.UtcNow.Add(retryAfter);
            }
            finally
            {
                _globalLock.Release();
            }
        }

        private static bool IsGlobalRateLimit(
            HttpResponseMessage response)
        {
            return TryGetHeader(
                       response,
                       "X-RateLimit-Global",
                       out var value) &&
                   value.Equals(
                       "true",
                       StringComparison.OrdinalIgnoreCase);
        }

        private static async Task<TimeSpan> GetRetryAfterAsync(
            HttpResponseMessage response,
            CancellationToken cancellationToken)
        {
            if (TryGetHeader(response, "Retry-After", out var retryAfterHeader) &&
                double.TryParse(
                    retryAfterHeader,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var retryAfterSeconds))
            {
                return TimeSpan.FromSeconds(retryAfterSeconds);
            }

            var body = await response.Content.ReadAsStringAsync(
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(body))
            {
                try
                {
                    using var json = JsonDocument.Parse(body);

                    if (json.RootElement.TryGetProperty(
                            "retry_after",
                            out var retryAfterProperty))
                    {
                        return TimeSpan.FromSeconds(
                            retryAfterProperty.GetDouble());
                    }
                }
                catch
                {
                    // Ignore unexpected response body.
                }
            }

            return TimeSpan.FromSeconds(1);
        }

        private static bool TryGetHeader(
            HttpResponseMessage response,
            string name,
            out string value)
        {
            if (response.Headers.TryGetValues(name, out var headerValues))
            {
                value = headerValues.FirstOrDefault() ?? string.Empty;
                return !string.IsNullOrWhiteSpace(value);
            }

            if (response.Content.Headers.TryGetValues(
                    name,
                    out var contentHeaderValues))
            {
                value = contentHeaderValues.FirstOrDefault() ?? string.Empty;
                return !string.IsNullOrWhiteSpace(value);
            }

            value = string.Empty;
            return false;
        }

        private sealed class BucketState
        {
            public SemaphoreSlim Lock { get; } = new(1, 1);

            public int Remaining { get; set; } = 1;

            public DateTimeOffset ResetAt { get; set; } =
                DateTimeOffset.MinValue;
        }
    }
}