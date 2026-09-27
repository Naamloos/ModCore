using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record GuildScheduledEvent
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("channel_id")]
        public Snowflake? ChannelId { get; set; }

        [JsonPropertyName("creator_id")]
        public Optional<Snowflake?> CreatorId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("description")]
        public Optional<string?> Description { get; set; }

        [JsonPropertyName("scheduled_start_time")]
        public DateTimeOffset ScheduledStartTime { get; set; }

        [JsonPropertyName("scheduled_end_time")]
        public DateTimeOffset? ScheduledEndTime { get; set; }

        [JsonPropertyName("privacy_level")]
        public int PrivacyLevel { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("entity_type")]
        public int EntityType { get; set; }

        [JsonPropertyName("entity_id")]
        public Snowflake? EntityId { get; set; }

        [JsonPropertyName("entity_metadata")]
        public GuildScheduledEventEntityMetadata? EntityMetadata { get; set; }

        [JsonPropertyName("creator")]
        public Optional<User> Creator { get; set; }

        [JsonPropertyName("user_count")]
        public Optional<int> UserCount { get; set; }

        [JsonPropertyName("image")]
        public Optional<string?> Image { get; set; }

        [JsonPropertyName("recurrence_rule")]
        public GuildScheduledEventRecurrenceRule? RecurrenceRule { get; set; }
    }
}
