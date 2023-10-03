using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities
{
    public record RoleTag
    {
        [JsonPropertyName("bot_id")]
        public Optional<Snowflake> BotId { get; set; }

        [JsonPropertyName("integration_id")]
        public Optional<Snowflake> IntegrationId { get; set; }

        [JsonPropertyName("premium_subscriber")]
        public IdiotBool PremiumSubscriber { get; set; }

        [JsonPropertyName("subscription_listing_id")]
        public Snowflake SubscriptionListingId { get; set; }

        [JsonPropertyName("available_for_purchase")]
        public IdiotBool AvailableForPurchase { get; set; }

        [JsonPropertyName("guild_connections")]
        public IdiotBool GuildConnections { get; set; }
    }
}