using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record EmbedAuthor
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public Optional<string> Url { get; set; }

        [JsonPropertyName("proxy_url")]
        public Optional<string> ProxyUrl { get; set; }

        [JsonPropertyName("icon_url")]
        public Optional<string> IconUrl { get; set; }

        [JsonPropertyName("proxy_icon_url")]
        public Optional<string> ProxyIconUrl { get; set; }
    }
}