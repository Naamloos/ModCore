using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record DynamicStringField
    {
        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("value")]
        public string Value { get; set; } = default!;
    }
}
