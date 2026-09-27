using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record AutoModerationRule
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("creator_id")]
        public Snowflake CreatorId { get; set; }

        [JsonPropertyName("event_type")]
        public int EventType { get; set; }

        [JsonPropertyName("trigger_type")]
        public int TriggerType { get; set; }

        [JsonPropertyName("trigger_metadata")]
        public TriggerMetadata TriggerMetadata { get; set; } = default!;

        [JsonPropertyName("actions")]
        public AutoModerationAction[] Actions { get; set; } = default!;

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("exempt_roles")]
        public Snowflake[] ExemptRoles { get; set; } = default!;

        [JsonPropertyName("exempt_channels")]
        public Snowflake[] ExemptChannels { get; set; } = default!;
    }
}
