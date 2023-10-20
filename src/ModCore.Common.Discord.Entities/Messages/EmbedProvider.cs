using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record EmbedProvider
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("url")]
        public Optional<string> Url { get; set; }
    }
}