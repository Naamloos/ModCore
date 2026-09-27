using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Components;

namespace ModCore.Common.Discord.Entities.Gateway
{
    public record ActivityButtons
    {
        [JsonPropertyName("label")]
        public string Label { get; set; } = default!;

        [JsonPropertyName("url")]
        public string Url { get; set; } = default!;
    }
}
