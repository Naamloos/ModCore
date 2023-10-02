using ModCore.Common.Discord.Gateway.Events;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Gateway.EventData.Incoming
{
    public record Hello : IPublishable
    {
        [JsonPropertyName("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }
    }
}
