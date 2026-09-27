using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Components
{
    public class TextInputInteractionResponse
    {
        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("custom_id")]
        public string CustomId { get; set; } = default!;

        [JsonPropertyName("value")]
        public string Value { get; set; } = default!;
    }
}
