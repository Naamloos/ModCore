using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Messages;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record InteractionCallbackResource
    {
        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("activity_instance")]
        public Optional<InteractionCallbackActivityInstanceResource> ActivityInstance { get; set; }

        [JsonPropertyName("message")]
        public Optional<Message> Message { get; set; }
    }
}
