using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Common.PubSub.Attributes
{
    /// <summary>
    /// Indicates to the pub/sub service which event channel a subscriber is subscribing to.
    /// </summary>
    public class EventChannelAttribute : Attribute
    {
        public EventChannels EventChannel { get; private set; }

        public EventChannelAttribute(EventChannels eventChannel)
        {
            this.EventChannel = eventChannel;
        }
    }
}
