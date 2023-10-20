using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record EmbedField
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("inline")]
        public Optional<bool> Inline { get; set; }
    }
}