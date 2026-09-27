using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Interactions;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<
            RestResponse<ApplicationRoleConnectionMetadata[]>
        > GetApplicationRoleConnectionMetadataRecordsAsync(
            Snowflake applicationId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ApplicationRoleConnectionMetadata[]>(
                HttpMethod.Get,
                $"applications/{EscapeRouteValue(applicationId)}/role-connections/metadata",
                $"applications/:application_id/role-connections/metadata",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<
            RestResponse<ApplicationRoleConnectionMetadata[]>
        > UpdateApplicationRoleConnectionMetadataRecordsAsync(
            Snowflake applicationId,
            ApplicationRoleConnectionMetadata[] body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ApplicationRoleConnectionMetadata[]>(
                HttpMethod.Put,
                $"applications/{EscapeRouteValue(applicationId)}/role-connections/metadata",
                $"applications/:application_id/role-connections/metadata",
                body,
                null,
                auditLogReason,
                cancellationToken
            );
    }
}
