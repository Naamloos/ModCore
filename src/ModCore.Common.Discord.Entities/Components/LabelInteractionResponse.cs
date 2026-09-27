using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Components
{
    public class LabelInteractionResponse
    {
        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("component")]
        public Component Component { get; set; } = default!;
    }
}
