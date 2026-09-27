using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record ActionMetadata
    {
        [JsonPropertyName("channel_id")]
        public Optional<Snowflake> ChannelId { get; set; }

        [JsonPropertyName("duration_seconds")]
        public Optional<int> DurationSeconds { get; set; }

        [JsonPropertyName("custom_message")]
        public Optional<string> CustomMessage { get; set; }
    }
}
