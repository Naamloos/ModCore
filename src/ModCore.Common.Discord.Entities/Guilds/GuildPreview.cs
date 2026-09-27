using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record GuildPreview
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }

        [JsonPropertyName("splash")]
        public string? Splash { get; set; }

        [JsonPropertyName("discovery_splash")]
        public string? DiscoverySplash { get; set; }

        [JsonPropertyName("emojis")]
        public Emoji[] Emojis { get; set; } = default!;

        [JsonPropertyName("features")]
        public string[] Features { get; set; } = default!;

        [JsonPropertyName("approximate_member_count")]
        public int ApproximateMemberCount { get; set; }

        [JsonPropertyName("approximate_presence_count")]
        public int ApproximatePresenceCount { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("stickers")]
        public Sticker[] Stickers { get; set; } = default!;
    }
}
