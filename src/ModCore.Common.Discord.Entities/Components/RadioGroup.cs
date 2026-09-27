using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Enums;

namespace ModCore.Common.Discord.Entities.Components
{
    public class RadioGroup : Component
    {
        [JsonPropertyName("type")]
        public override ComponentType Type { get; set; } = (ComponentType)21;

        [JsonPropertyName("custom_id")]
        public string CustomId { get; set; } = default!;

        [JsonPropertyName("options")]
        public RadioGroupOption[] Options { get; set; } = default!;

        [JsonPropertyName("required")]
        public Optional<bool> Required { get; set; }
    }
}
