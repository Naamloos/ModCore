using ModCore.Common.Discord.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities.Components
{
    public class Thumbnail : Component
    {
        public override ComponentType Type { get; set; } = ComponentType.Thumbnail;

        [JsonPropertyName("media")]
        public UnfurledMediaItem Media { get; set; }

        [JsonPropertyName("description")]
        public Optional<string> Description { get; set; } = Optional<string>.None;

        [JsonPropertyName("spoiler")]
        public Optional<bool> Spoiler { get; set; } = Optional<bool>.None;
    }
}
