using Microsoft.Extensions.DependencyInjection;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Gateway.EventData.Outgoing;
using ModCore.Common.Discord.Gateway.Events;
using System.Reflection;

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

        public void SubscribeEvents(Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(ISubscriber).IsAssignableFrom(t));
            foreach (var type in types)
            {
                if (type.IsGenericType)
                {
                    var genericType = type.GetGenericTypeDefinition();
                    if (genericType == typeof(ISubscriber<>))
                    {
                        subscribers.Add(type);
                    }
                }
                else
                {
                    subscribers.Add(type);
                }
            }
        }
    }
}
