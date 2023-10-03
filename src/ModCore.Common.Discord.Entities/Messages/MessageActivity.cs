using ModCore.Common.Discord.Entities.Enums;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record MessageActivity
    {
        [JsonPropertyName("type")]
        public MessageActivityType Type { get; set; }

        [JsonPropertyName("party_id")]
        public Optional<string> PartyId { get; set; }
    }
}