using ModCore.Common.Discord.Entities.Enums;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Components
{
    public class Separator : Component
    {
        [JsonPropertyName("type")]
        public override ComponentType Type { get; set; } = ComponentType.Separator;

        /// <summary>
        /// Controls whether the line itself renders visually or acts purely as whitespace spacing.
        /// </summary>
        [JsonPropertyName("divider")]
        public bool Divider { get; set; } = true;

        /// <summary>
        /// Explicit spacing layout metric config adjustments.
        /// </summary>
        [JsonPropertyName("spacing")]
        public Optional<int> Spacing { get; set; }
    }
}