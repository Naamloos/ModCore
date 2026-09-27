using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record GuildTemplate
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = default!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("usage_count")]
        public int UsageCount { get; set; }

        [JsonPropertyName("creator_id")]
        public Snowflake CreatorId { get; set; }

        [JsonPropertyName("creator")]
        public User Creator { get; set; } = default!;

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonPropertyName("source_guild_id")]
        public Snowflake SourceGuildId { get; set; }

        [JsonPropertyName("serialized_source_guild")]
        public Guild SerializedSourceGuild { get; set; } = default!;

        [JsonPropertyName("is_dirty")]
        public bool? IsDirty { get; set; }
    }
}
