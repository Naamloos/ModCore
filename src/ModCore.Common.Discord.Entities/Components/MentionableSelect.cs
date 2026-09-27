using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Enums;

namespace ModCore.Common.Discord.Entities.Components
{
    public class MentionableSelect : Component
    {
        [JsonPropertyName("type")]
        public override ComponentType Type { get; set; } = (ComponentType)7;

        [JsonPropertyName("custom_id")]
        public string CustomId { get; set; } = default!;

        [JsonPropertyName("placeholder")]
        public Optional<string> Placeholder { get; set; }

        [JsonPropertyName("default_values")]
        public Optional<SelectDefaultValue[]> DefaultValues { get; set; }

        [JsonPropertyName("min_values")]
        public Optional<int> MinValues { get; set; }

        [JsonPropertyName("max_values")]
        public Optional<int> MaxValues { get; set; }

        [JsonPropertyName("required")]
        public Optional<bool> Required { get; set; }

        [JsonPropertyName("disabled")]
        public Optional<bool> Disabled { get; set; }
    }
}
