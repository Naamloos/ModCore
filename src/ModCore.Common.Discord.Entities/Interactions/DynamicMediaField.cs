using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record DynamicMediaField
    {
        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("value")]
        public Media Value { get; set; } = default!;
    }
}
