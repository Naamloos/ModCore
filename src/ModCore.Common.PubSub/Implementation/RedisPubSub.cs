using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ModCore.Common.PubSub.Implementation
{
    public sealed class RedisPubSub : IPubSub
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<RedisPubSub> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public RedisPubSub(
            IConnectionMultiplexer redis,
            ILogger<RedisPubSub> logger, JsonSerializerOptions? options = null)
        {
            _redis = redis;
            _logger = logger;

            _jsonOptions = options ?? new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task PublishAsync<T>(
            string channel,
            T message,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var subscriber = _redis.GetSubscriber();

            var payload = JsonSerializer.Serialize(message, _jsonOptions);

            await subscriber.PublishAsync(
                RedisChannel.Literal(channel),
                payload);
        }

        public async Task SubscribeAsync<T>(
            string channel,
            Func<T, CancellationToken, Task> handler,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var subscriber = _redis.GetSubscriber();

            await subscriber.SubscribeAsync(
                RedisChannel.Literal(channel),
                async (_, value) =>
                {
                    try
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return;

                        var message = JsonSerializer.Deserialize<T>(
                            value.ToString()!,
                            _jsonOptions);

                        if (message is null)
                            return;

                        await handler(message, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        // App is shutting down.
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Failed to handle Redis message on channel {Channel}",
                            channel);
                    }
                });
        }

        public async Task UnsubscribeAsync(
            string channel,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var subscriber = _redis.GetSubscriber();

            await subscriber.UnsubscribeAsync(
                RedisChannel.Literal(channel));
        }
    }
}
