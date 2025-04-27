using ModCore.Common.Discord.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities.Components
{
    public class TextDisplay : Component
    {
        [JsonPropertyName("type")]
        public override ComponentType Type { get; set; } = ComponentType.TextDisplay;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }
}
