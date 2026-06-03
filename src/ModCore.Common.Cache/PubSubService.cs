using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Text.Json;

namespace ModCore.Common.Cache
{
    public interface IPubSubService
    {
        Task PublishAsync<T>(string channel, T message);
        Task SubscribeAsync<T>(string channel, Action<T, IServiceProvider> handler);
    }

    public class PubSubService : IPubSubService
    {
        private readonly IConnectionMultiplexer? _redis;
        private readonly IServiceProvider _services;

        public PubSubService(IConnectionMultiplexer? redis, IServiceProvider services)
        {
            _redis = redis;
            _services = services;
        }

        public async Task PublishAsync<T>(string channel, T message)
        {
            if(_redis == null)
            {
                // no-op if no redis.
                return;
            }

            var pub = _redis.GetSubscriber();
            var payload = JsonSerializer.Serialize(message);
            await pub.PublishAsync(channel, payload);
        }

        public async Task SubscribeAsync<T>(string channel, Action<T, IServiceProvider> handler)
        {
            if (_redis == null)
            {
                // no-op if no redis.
                return;
            }

            var sub = _redis.GetSubscriber();
            await sub.SubscribeAsync(channel, (ch, message) =>
            {
                var deserialized = JsonSerializer.Deserialize<T>(message.ToString());
                if (deserialized != null)
                {
                    handler(deserialized, _services);
                }
            });
        }
    }
}