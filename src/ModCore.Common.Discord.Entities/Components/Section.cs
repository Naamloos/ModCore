using ModCore.Common.Discord.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities.Components
{
    public class Section : Component
    {
        [JsonPropertyName("type")]
        public override ComponentType Type { get; set; } = ComponentType.Section;

        [JsonPropertyName("components")]
        public List<TextDisplay> Components { get; set; } = new List<TextDisplay>();

        [JsonPropertyName("accessory")]
        public Component Accessory { get; set; }
    }
}
