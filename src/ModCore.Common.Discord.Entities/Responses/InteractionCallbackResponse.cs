using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Interactions;

namespace ModCore.Common.Discord.Entities.Responses
{
    public record InteractionCallbackResponse
    {
        [JsonPropertyName("interaction")]
        public InteractionCallback Interaction { get; set; } = default!;

        [JsonPropertyName("resource")]
        public Optional<InteractionCallbackResource> Resource { get; set; }
    }
}
