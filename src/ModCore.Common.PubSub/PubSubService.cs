using StackExchange.Redis;
using System.Text.Json;

namespace ModCore.Common.PubSub
{
    public interface IPubSubService
    {
        Task PublishAsync<T>(string channel, T message);
        Task SubscribeAsync<T>(string channel, Action<T> handler);
    }

    public class PubSubService : IPubSubService
    {
        private readonly IConnectionMultiplexer _redis;

        public PubSubService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task PublishAsync<T>(string channel, T message)
        {
            var pub = _redis.GetSubscriber();
            var payload = JsonSerializer.Serialize(message);
            await pub.PublishAsync(channel, payload);
        }

        public async Task SubscribeAsync<T>(string channel, Action<T> handler)
        {
            var sub = _redis.GetSubscriber();
            await sub.SubscribeAsync(channel, (ch, message) =>
            {
                var deserialized = JsonSerializer.Deserialize<T>(message.ToString());
                if (deserialized != null)
                {
                    handler(deserialized);
                }
            });
        }
    }
}