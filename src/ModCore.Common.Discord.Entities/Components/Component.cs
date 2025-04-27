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
    [JsonDerivedType(typeof(Container))]
    [JsonDerivedType(typeof(StringSelect))]
    [JsonDerivedType(typeof(TextDisplay))]
    [JsonDerivedType(typeof(Section))]
    [JsonDerivedType(typeof(Thumbnail))]
    public class Component
    {
        [JsonPropertyName("type")]
        public virtual ComponentType Type { get; set; }

        [JsonPropertyName("id")]
        public Optional<int> Id { get; set; } = Optional<int>.None;
    }
}
