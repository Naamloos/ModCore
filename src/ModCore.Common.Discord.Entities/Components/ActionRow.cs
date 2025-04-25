using ModCore.Common.Discord.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities.Components
{
    public class ActionRow : Component
    {
        [JsonPropertyName("type")]
        public ComponentType Type { get; set; } = ComponentType.ActionRow;

        [JsonPropertyName("components")]
        public List<Component> Components { get; set; } = new List<Component>();
    }
}
