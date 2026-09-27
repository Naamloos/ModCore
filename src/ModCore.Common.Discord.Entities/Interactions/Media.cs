using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record Media
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = default!;
    }
}
