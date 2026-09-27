using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record MessageComponentData
    {
        [JsonPropertyName("custom_id")]
        public string CustomId { get; set; } = default!;

        [JsonPropertyName("component_type")]
        public int ComponentType { get; set; }

        [JsonPropertyName("values")]
        public Optional<string[]> Values { get; set; }

        [JsonPropertyName("resolved")]
        public Optional<ResolvedDataStructure> Resolved { get; set; }
    }
}
