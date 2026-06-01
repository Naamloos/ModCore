using ModCore.Common.Discord.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities.Components
{
    public class Container : ComponentWithChildren
    {
        [JsonPropertyName("type")]
        public override ComponentType Type { get; set; } = ComponentType.Container;

        [JsonPropertyName("accent_color")]
        public Optional<int?> AccentColor { get; set; } = Optional<int?>.None;

        [JsonPropertyName("spoiler")]
        public Optional<bool> Spoiler { get; set; } = Optional<bool>.None;
    }
}
