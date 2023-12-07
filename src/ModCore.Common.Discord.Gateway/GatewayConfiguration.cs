using Microsoft.Extensions.DependencyInjection;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Gateway.EventData.Outgoing;
using ModCore.Common.Discord.Gateway.Events;

namespace ModCore.Common.Discord.Gateway
{
    public record GatewayConfiguration
    {
        public Intents Intents { get; set; } = Intents.AllUnprivileged;
        public string GatewayUrl { get; set; } = "gateway.discord.gg";
        public IServiceProvider Services { get; set; } = new ServiceCollection().BuildServiceProvider(); // placeholder dummy
        public Optional<Activity> Activity { get; set; } = Optional<Activity>.None;

        internal List<Type> subscribers { get; set; } = new List<Type>();
        public void SubscribeEvents<T>() where T : ISubscriber
        {
            var type = typeof(T);
            subscribers.Add(type);
        }
    }
}
