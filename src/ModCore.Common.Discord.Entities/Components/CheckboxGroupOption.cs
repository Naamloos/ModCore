using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Components
{
    public class CheckboxGroupOption
    {
        [JsonPropertyName("value")]
        public string Value { get; set; } = default!;

        [JsonPropertyName("label")]
        public string Label { get; set; } = default!;

        [JsonPropertyName("description")]
        public Optional<string> Description { get; set; }

        [JsonPropertyName("default")]
        public Optional<bool> Default { get; set; }
    }
}
