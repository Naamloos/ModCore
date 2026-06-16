using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.PubSub.Attributes;
using ModCore.Common.PubSub.Implementation;
using ModCore.Common.PubSub.Payloads;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ModCore.Common.PubSub
{
    public class PubSubService
    {
        private readonly IPubSub _pubSubImplementation;
        private readonly IDictionary<Type, string> channelNames;

        public PubSubService(IPubSub pubSubImplementation)
        {
            _pubSubImplementation = pubSubImplementation;
            channelNames = new Dictionary<Type, string>();
        }

        public Task PublishAsync<T>(
            T message,
            CancellationToken cancellationToken = default
        ) where T : IPubSubPayload
        {
            var channelName = getChannelName<T>();
            return _pubSubImplementation.PublishAsync(channelName, message, cancellationToken);
        }

        public Task SubscribeAsync<T>(
            Func<T, CancellationToken, Task> handler,
            CancellationToken cancellationToken = default
        ) where T : IPubSubPayload
        {
            var channelName = getChannelName<T>();
            return _pubSubImplementation.SubscribeAsync(channelName, handler, cancellationToken);
        }

        public Task UnsubscribeAsync<T>(
            CancellationToken cancellationToken = default
        ) where T : IPubSubPayload
        {
            var channelName = getChannelName<T>();
            return _pubSubImplementation.UnsubscribeAsync(channelName, cancellationToken);
        }

        private string getChannelName<T>() where T : IPubSubPayload
        {
            var type = typeof(T);

            if (channelNames.ContainsKey(type))
            {
                return channelNames[type];
            }

            var channelAttribute = type.GetCustomAttribute<EventChannelAttribute>();

            if(channelAttribute?.EventChannel == null)
            {
                throw new NotImplementedException("PubSub payloads must be decorated with the EventChannel attribute");
            }

            var name = Enum.GetName<EventChannels>(channelAttribute.EventChannel);
            channelNames.Add(type, name!);

            return name!;
        }
    }
}
