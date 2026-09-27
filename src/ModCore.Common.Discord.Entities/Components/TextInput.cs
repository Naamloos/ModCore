using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Enums;

namespace ModCore.Common.Discord.Entities.Components
{
    public class TextInput : Component
    {
        [JsonPropertyName("type")]
        public override ComponentType Type { get; set; } = (ComponentType)4;

        [JsonPropertyName("custom_id")]
        public string CustomId { get; set; } = default!;

        [JsonPropertyName("style")]
        public int Style { get; set; }

        [JsonPropertyName("min_length")]
        public Optional<int> MinLength { get; set; }

        [JsonPropertyName("max_length")]
        public Optional<int> MaxLength { get; set; }

        [JsonPropertyName("required")]
        public Optional<bool> Required { get; set; }

        [JsonPropertyName("value")]
        public Optional<string> Value { get; set; }

        [JsonPropertyName("placeholder")]
        public Optional<string> Placeholder { get; set; }
    }
}
