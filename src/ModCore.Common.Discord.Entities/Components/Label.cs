using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Enums;

namespace ModCore.Common.Discord.Entities.Components
{
    public class Label : Component
    {
        [JsonPropertyName("type")]
        public override ComponentType Type { get; set; } = (ComponentType)18;

        [JsonPropertyName("label")]
        public string LabelValue { get; set; } = default!;

        [JsonPropertyName("description")]
        public Optional<string> Description { get; set; }

        [JsonPropertyName("component")]
        public Component Component { get; set; } = default!;
    }
}
