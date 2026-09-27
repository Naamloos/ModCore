using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Gateway
{
    public record ChannelInfoChannel
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("status")]
        public Optional<string?> Status { get; set; }

        [JsonPropertyName("voice_start_time")]
        public Optional<int?> VoiceStartTime { get; set; }
    }
}
