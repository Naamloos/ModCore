using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ModCore.Common.Discord.Gateway.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Gateway
{
    public record GatewayConfiguration
    {
        public Intents Intents { get; set; } = Intents.AllUnprivileged;
        public string GatewayUrl { get; set; } = "gateway.discord.gg";
        public IServiceProvider Services { get; set; } = new ServiceCollection().BuildServiceProvider(); // placeholder dummy

        internal List<Type> subscribers { get; set; } = new List<Type>();
        public void SubscribeEvents<T>() where T : ISubscriber
        {
            var type = typeof(T);
            subscribers.Add(type);
        }
    }
}
