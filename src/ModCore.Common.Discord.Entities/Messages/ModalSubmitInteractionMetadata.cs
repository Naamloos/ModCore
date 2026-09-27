using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record ModalSubmitInteractionMetadata
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("user")]
        public User User { get; set; } = default!;

        [JsonPropertyName("authorizing_integration_owners")]
        public Dictionary<string, Snowflake> AuthorizingIntegrationOwners { get; set; }

        [JsonPropertyName("original_response_message_id")]
        public Optional<Snowflake> OriginalResponseMessageId { get; set; }

        [JsonPropertyName("triggering_interaction_metadata")]
        public ApplicationCommandInteractionMetadata TriggeringInteractionMetadata { get; set; } =
            default!;
    }
}
