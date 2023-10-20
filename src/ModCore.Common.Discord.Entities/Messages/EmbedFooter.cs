using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record EmbedFooter
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("icon_url")]
        public Optional<string> IconUrl { get; set; }

        [JsonPropertyName("proxy_icon_url")]
        public Optional<string> ProxyIconUrl { get; set; }
    }
}