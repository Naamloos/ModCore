using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Components;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record ModalSubmitData
    {
        [JsonPropertyName("custom_id")]
        public string CustomId { get; set; } = default!;

        [JsonPropertyName("components")]
        public Component[] Components { get; set; } = default!;

        [JsonPropertyName("resolved")]
        public Optional<ResolvedDataStructure> Resolved { get; set; }
    }
}
