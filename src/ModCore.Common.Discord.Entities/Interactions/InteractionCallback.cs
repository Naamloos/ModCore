using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record InteractionCallback
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("activity_instance_id")]
        public Optional<string> ActivityInstanceId { get; set; }

        [JsonPropertyName("response_message_id")]
        public Optional<Snowflake> ResponseMessageId { get; set; }

        [JsonPropertyName("response_message_loading")]
        public Optional<bool> ResponseMessageLoading { get; set; }

        [JsonPropertyName("response_message_ephemeral")]
        public Optional<bool> ResponseMessageEphemeral { get; set; }
    }
}
