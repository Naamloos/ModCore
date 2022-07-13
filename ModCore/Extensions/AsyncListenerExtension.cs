using DSharpPlus;
using ModCore.Extensions.AsyncListeners.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace ModCore.Extensions
{
    public class AsyncListenerExtension : BaseExtension
    {
        private IServiceProvider services;
        private DiscordClient client;

        public AsyncListenerExtension(IServiceProvider services)
        {
            this.services = services;
        }

        protected override void Setup(DiscordClient client)
        {
            this.client = client;
        }

        public void RegisterListeners(Assembly assembly)
        {
            var methods = assembly.DefinedTypes
                .SelectMany(x => x.GetMethods())
                .Where(x => x.GetCustomAttribute<AsyncListenerAttribute>() != null)
                .Select(x => new ListenerMethod { Attribute = x.GetCustomAttribute<AsyncListenerAttribute>(), Method = x });

            foreach (var listener in methods)
            {
                listener.Attribute.Register(this.client, listener.Method, this.services);
            }
        }
    }

    class ListenerMethod
    {
        public MethodInfo Method { get; internal set; }
        public AsyncListenerAttribute Attribute { get; internal set; }
    }
}
