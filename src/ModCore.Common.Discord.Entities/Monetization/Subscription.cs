using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Monetization
{
    public record Subscription
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("user_id")]
        public Snowflake UserId { get; set; }

        [JsonPropertyName("sku_ids")]
        public Snowflake[] SkuIds { get; set; } = default!;

        [JsonPropertyName("entitlement_ids")]
        public Snowflake[] EntitlementIds { get; set; } = default!;

        [JsonPropertyName("renewal_sku_ids")]
        public Snowflake[]? RenewalSkuIds { get; set; }

        [JsonPropertyName("current_period_start")]
        public DateTimeOffset CurrentPeriodStart { get; set; }

        [JsonPropertyName("current_period_end")]
        public DateTimeOffset CurrentPeriodEnd { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("canceled_at")]
        public DateTimeOffset? CanceledAt { get; set; }

        [JsonPropertyName("country")]
        public Optional<string> Country { get; set; }
    }
}
