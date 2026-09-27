using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Requests;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<Entitlement[]>> ListEntitlementsAsync(
            Snowflake applicationId,
            ListEntitlementsQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Entitlement[]>(
                HttpMethod.Get,
                $"applications/{EscapeRouteValue(applicationId)}/entitlements",
                $"applications/:application_id/entitlements",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Entitlement>> GetEntitlementAsync(
            Snowflake applicationId,
            Snowflake entitlementId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Entitlement>(
                HttpMethod.Get,
                $"applications/{EscapeRouteValue(applicationId)}/entitlements/{EscapeRouteValue(entitlementId)}",
                $"applications/:application_id/entitlements/:entitlement_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> ConsumeAnEntitlementAsync(
            Snowflake applicationId,
            Snowflake entitlementId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Post,
                $"applications/{EscapeRouteValue(applicationId)}/entitlements/{EscapeRouteValue(entitlementId)}/consume",
                $"applications/:application_id/entitlements/:entitlement_id/consume",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Entitlement>> CreateTestEntitlementAsync(
            Snowflake applicationId,
            CreateTestEntitlementRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Entitlement>(
                HttpMethod.Post,
                $"applications/{EscapeRouteValue(applicationId)}/entitlements",
                $"applications/:application_id/entitlements",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteTestEntitlementAsync(
            Snowflake applicationId,
            Snowflake entitlementId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"applications/{EscapeRouteValue(applicationId)}/entitlements/{EscapeRouteValue(entitlementId)}",
                $"applications/:application_id/entitlements/:entitlement_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );
    }
}
