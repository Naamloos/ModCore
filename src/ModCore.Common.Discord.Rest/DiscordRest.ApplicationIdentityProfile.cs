using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Requests;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<
            RestResponse<ApplicationIdentityProfile>
        > UpdateApplicationIdentityProfileAsync(
            Snowflake applicationId,
            Snowflake userId,
            string providerIssuedUserId,
            UpdateApplicationIdentityProfileRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ApplicationIdentityProfile>(
                HttpMethod.Patch,
                $"applications/{EscapeRouteValue(applicationId)}/users/{EscapeRouteValue(userId)}/identities/{EscapeRouteValue(providerIssuedUserId)}/profile",
                $"applications/:application_id/users/:user_id/identities/:provider_issued_user_id/profile",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<
            RestResponse<ApplicationIdentityProfile>
        > GetApplicationIdentityProfileAsync(
            Snowflake applicationId,
            Snowflake userId,
            string providerIssuedUserId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ApplicationIdentityProfile>(
                HttpMethod.Get,
                $"applications/{EscapeRouteValue(applicationId)}/users/{EscapeRouteValue(userId)}/identities/{EscapeRouteValue(providerIssuedUserId)}/profile",
                $"applications/:application_id/users/:user_id/identities/:provider_issued_user_id/profile",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<ApplicationIdentities>> GetApplicationIdentitiesByUserIDAsync(
            Snowflake userId,
            Snowflake applicationId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ApplicationIdentities>(
                HttpMethod.Get,
                $"users/{EscapeRouteValue(userId)}/application-identities/{EscapeRouteValue(applicationId)}",
                $"users/:user_id/application-identities/:application_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<
            RestResponse<ApplicationIdentities>
        > GetApplicationIdentitiesByExternalIDAsync(
            Snowflake applicationId,
            string providerType,
            string providerIssuedUserId,
            GetApplicationIdentitiesByExternalIDQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ApplicationIdentities>(
                HttpMethod.Get,
                $"applications/{EscapeRouteValue(applicationId)}/application-identities/{EscapeRouteValue(providerType)}/{EscapeRouteValue(providerIssuedUserId)}",
                $"applications/:application_id/application-identities/:provider_type/:provider_issued_user_id",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteApplicationIdentityAsync(
            Snowflake userId,
            Snowflake applicationId,
            string providerType,
            string providerIssuedUserId,
            DeleteApplicationIdentityRequest? body = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Post,
                $"users/{EscapeRouteValue(userId)}/application-identities/{EscapeRouteValue(applicationId)}/{EscapeRouteValue(providerType)}/{EscapeRouteValue(providerIssuedUserId)}/delete",
                $"users/:user_id/application-identities/:application_id/:provider_type/:provider_issued_user_id/delete",
                body,
                null,
                auditLogReason,
                cancellationToken
            );
    }
}
