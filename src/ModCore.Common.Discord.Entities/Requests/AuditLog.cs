using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record GetGuildAuditLogQuery
    {
        [JsonPropertyName("user_id")]
        public Optional<Snowflake> UserId { get; set; }

        [JsonPropertyName("action_type")]
        public Optional<int> ActionType { get; set; }

        [JsonPropertyName("before")]
        public Optional<Snowflake> Before { get; set; }

        [JsonPropertyName("after")]
        public Optional<Snowflake> After { get; set; }

        [JsonPropertyName("limit")]
        public Optional<int> Limit { get; set; }
    }
}
