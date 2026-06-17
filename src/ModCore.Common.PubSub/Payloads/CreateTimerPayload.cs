using ModCore.Common.PubSub.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ModCore.Common.PubSub.Payloads
{
    /// <summary>
    /// Lets the timer service know that a new timer was created and it should check if it needs to update its timers.
    /// </summary>
    [EventChannel(EventChannels.TimerCreated)]
    public record CreateTimerPayload : IPubSubPayload
    {
        [JsonPropertyName("timer_id")]
        public long TimerId { get; init; }
    }
}
