using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Monetization;
using ModCore.Common.Discord.Entities.Requests;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<Subscription[]>> ListSKUSubscriptionsAsync(
            Snowflake skuId,
            ListSKUSubscriptionsQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Subscription[]>(
                HttpMethod.Get,
                $"skus/{EscapeRouteValue(skuId)}/subscriptions",
                $"skus/:sku_id/subscriptions",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Subscription>> GetSKUSubscriptionAsync(
            Snowflake skuId,
            Snowflake subscriptionId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Subscription>(
                HttpMethod.Get,
                $"skus/{EscapeRouteValue(skuId)}/subscriptions/{EscapeRouteValue(subscriptionId)}",
                $"skus/:sku_id/subscriptions/:subscription_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );
    }
}
