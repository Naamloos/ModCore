using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Messages;
using System.Text.Json.Serialization;

namespace ModCore.Services.Web.RequestBodies
{
    public class WelcomeSettingsUpdate
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("message_payload")]
        public CreateMessage MessagePayload { get; set; } = new CreateMessage();

        [JsonPropertyName("channel_id")]
        public ulong ChannelId { get; set; }
    }
}
