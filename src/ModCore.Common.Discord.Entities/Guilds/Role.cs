using ModCore.Common.Discord.Entities.Enums;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record Role
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("color")]
        public int Color { get; set; }

        [JsonPropertyName("hoist")]
        public bool Hoist { get; set; }

        [JsonPropertyName("icon")]
        public Optional<string?> Icon { get; set; }

        [JsonPropertyName("unicode_emoji")]
        public Optional<string?> UnicodeEmoji { get; set; }

        [JsonPropertyName("position")]
        public int Position { get; set; }

        [JsonPropertyName("permissions")]
        public Permissions Permissions { get; set; }

        [JsonPropertyName("managed")]
        public bool Managed { get; set; }

        [JsonPropertyName("mentionable")]
        public bool Mentionable { get; set; }

        // TODO FIX
        //[JsonPropertyName("tags")]
        //public Optional<RoleTag[]> Tags { get; set; }

        //[JsonPropertyName("flags")]
        //public RoleFlags Flags { get; set; }
    }
}