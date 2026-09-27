using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record EmbedVideo
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("proxy_url")]
        public Optional<string> ProxyUrl { get; set; }

        [JsonPropertyName("height")]
        public Optional<int> Height { get; set; }

        [JsonPropertyName("width")]
        public Optional<int> Width { get; set; }

        [JsonPropertyName("content_type")]
        public Optional<string> ContentType { get; set; }

        [JsonPropertyName("placeholder")]
        public Optional<string> Placeholder { get; set; }

        [JsonPropertyName("placeholder_version")]
        public Optional<int> PlaceholderVersion { get; set; }

        [JsonPropertyName("description")]
        public Optional<string> Description { get; set; }

        [JsonPropertyName("flags")]
        public Optional<long> Flags { get; set; }
    }
}
