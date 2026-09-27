using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record CreateInteractionResponseQuery
    {
        [JsonPropertyName("with_response")]
        public Optional<bool> WithResponse { get; set; }
    }
}
