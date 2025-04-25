using ModCore.Common.Discord.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities.Components
{
    [JsonDerivedType(typeof(ActionRow))]
    [JsonDerivedType(typeof(Button))]
    public class Component
    {
        [JsonPropertyName("type")]
        public ComponentType Type { get; set; }

        [JsonPropertyName("id")]
        public int? Id { get; set; } = Random.Shared.Next();
    }
}
