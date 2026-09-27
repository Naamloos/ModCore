using System.Text.Json.Serialization;
using ModCore.Common.Discord.Gateway.Events;

namespace ModCore.Common.Discord.Gateway.EventData.Incoming
{
    public record Hello : IPublishable
    {
        [JsonPropertyName("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }
    }
}
