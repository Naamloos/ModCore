using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Enums;

namespace ModCore.Common.Discord.Entities.Components
{
    public class File : Component
    {
        [JsonPropertyName("type")]
        public override ComponentType Type { get; set; } = (ComponentType)13;

        [JsonPropertyName("spoiler")]
        public Optional<bool> Spoiler { get; set; }

        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("size")]
        public Optional<int> Size { get; set; }
    }
}
