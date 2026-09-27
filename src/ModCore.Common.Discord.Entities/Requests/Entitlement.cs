using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record ListEntitlementsQuery
    {
        [JsonPropertyName("user_id")]
        public Optional<Snowflake> UserId { get; set; }

        [JsonPropertyName("sku_ids")]
        public Optional<Snowflake> SkuIds { get; set; }

        [JsonPropertyName("before")]
        public Optional<Snowflake> Before { get; set; }

        [JsonPropertyName("after")]
        public Optional<Snowflake> After { get; set; }

        [JsonPropertyName("limit")]
        public Optional<int> Limit { get; set; }

        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }

        [JsonPropertyName("exclude_ended")]
        public Optional<bool> ExcludeEnded { get; set; }

        [JsonPropertyName("exclude_deleted")]
        public Optional<bool> ExcludeDeleted { get; set; }
    }

    public record CreateTestEntitlementRequest
    {
        [JsonPropertyName("sku_id")]
        public Optional<string> SkuId { get; set; }

        [JsonPropertyName("owner_id")]
        public Optional<string> OwnerId { get; set; }

        [JsonPropertyName("owner_type")]
        public Optional<int> OwnerType { get; set; }
    }
}
