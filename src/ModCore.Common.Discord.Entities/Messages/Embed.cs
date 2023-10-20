using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record Embed
    {
        [JsonPropertyName("title")]
        public Optional<string> Title { get; set; }

        [JsonPropertyName("type")]
        public Optional<string> Type { get; set; }

        [JsonPropertyName("description")]
        public Optional<string> Description { get; set; }

        [JsonPropertyName("url")]
        public Optional<string> Url { get; set; }

        [JsonPropertyName("timestamp")]
        public Optional<DateTimeOffset> Timestamp { get; set; }

        [JsonPropertyName("color")]
        public Optional<int> Color { get; set; }

        [JsonPropertyName("footer")]
        public Optional<EmbedFooter> Footer { get; set; }

        [JsonPropertyName("image")]
        public Optional<EmbedImage> Image { get; set; }

        [JsonPropertyName("thumbnail")]
        public Optional<EmbedThumbnail> Thumbnail { get; set; }

        [JsonPropertyName("video")]
        public Optional<EmbedVideo> Video { get; set; }

        [JsonPropertyName("provider")]
        public Optional<EmbedProvider> Provider { get; set; }

        [JsonPropertyName("author")]
        public Optional<EmbedAuthor> Author { get; set; }

        [JsonPropertyName("fields")]
        public Optional<List<EmbedField>> Fields { get; set; }
    }
}