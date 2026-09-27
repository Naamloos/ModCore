using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Components;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record InteractionModalData : InteractionResponseData
    {
        [JsonPropertyName("custom_id")]
        public string CustomId { get; set; } = default!;

        [JsonPropertyName("title")]
        public string Title { get; set; } = default!;

        [JsonPropertyName("components")]
        public Component[] Components { get; set; } = default!;
    }
}
