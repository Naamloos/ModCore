using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Gateway
{
    public record ActivityEmoji
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("id")]
        public Optional<Snowflake> Id { get; set; }

        [JsonPropertyName("animated")]
        public Optional<bool> Animated { get; set; }
    }
}
