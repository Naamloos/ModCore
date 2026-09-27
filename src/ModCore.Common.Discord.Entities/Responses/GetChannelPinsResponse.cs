using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Messages;

namespace ModCore.Common.Discord.Entities.Responses
{
    public record GetChannelPinsResponse
    {
        [JsonPropertyName("items")]
        public MessagePin[] Items { get; set; } = default!;

        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }
    }
}
