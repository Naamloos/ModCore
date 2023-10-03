using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Channels
{
    public record Tag
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("moderated")]
        public bool Moderated { get; set; }

        [JsonPropertyName("emoji_id")]
        public Snowflake? EmojiId { get; set; }

        [JsonPropertyName("emoji_name")]
        public string? EmojiName { get; set; }
    }
}