using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record ListSKUSubscriptionsQuery
    {
        [JsonPropertyName("before")]
        public Optional<Snowflake> Before { get; set; }

        [JsonPropertyName("after")]
        public Optional<Snowflake> After { get; set; }

        [JsonPropertyName("limit")]
        public Optional<int> Limit { get; set; }

        [JsonPropertyName("user_id")]
        public Optional<Snowflake> UserId { get; set; }
    }
}
