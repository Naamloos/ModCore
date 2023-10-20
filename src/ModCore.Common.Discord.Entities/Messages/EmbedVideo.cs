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
    }
}