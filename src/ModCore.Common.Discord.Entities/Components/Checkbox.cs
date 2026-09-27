using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Enums;

namespace ModCore.Common.Discord.Entities.Components
{
    public class Checkbox : Component
    {
        [JsonPropertyName("type")]
        public override ComponentType Type { get; set; } = (ComponentType)23;

        [JsonPropertyName("custom_id")]
        public string CustomId { get; set; } = default!;

        [JsonPropertyName("default")]
        public Optional<bool> Default { get; set; }
    }
}
