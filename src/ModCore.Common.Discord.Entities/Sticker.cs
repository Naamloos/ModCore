using ModCore.Common.Discord.Entities.Enums;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities
{
    public record Sticker
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("pack_id")]
        public Optional<Snowflake> PackId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("tags")]
        public string Tags { get; set; }

        [JsonPropertyName("type")]
        public StickerType Type { get; set; }

        [JsonPropertyName("format_type")]
        public StickerFormat FormatType { get; set; }

        [JsonPropertyName("available")]
        public Optional<bool> Available { get; set; }

        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }

        [JsonPropertyName("user")]
        public Optional<User> User { get; set; }

        [JsonPropertyName("sort_value")]
        public Optional<int> SortValue { get; set; }
    }
}