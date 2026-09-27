using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Voice
{
    public record StageInstance
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; set; }

        [JsonPropertyName("topic")]
        public string Topic { get; set; } = default!;

        [JsonPropertyName("privacy_level")]
        public int PrivacyLevel { get; set; }

        [JsonPropertyName("discoverable_disabled")]
        public bool DiscoverableDisabled { get; set; }

        [JsonPropertyName("guild_scheduled_event_id")]
        public Snowflake? GuildScheduledEventId { get; set; }
    }
}
