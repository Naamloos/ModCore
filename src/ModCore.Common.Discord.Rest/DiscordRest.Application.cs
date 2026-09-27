using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Requests;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<Application>> GetCurrentApplicationAsync(
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Application>(
                HttpMethod.Get,
                $"applications/@me",
                $"applications/@me",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Application>> EditCurrentApplicationAsync(
            EditCurrentApplicationRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Application>(
                HttpMethod.Patch,
                $"applications/@me",
                $"applications/@me",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<ActivityInstance>> GetApplicationActivityInstanceAsync(
            Snowflake applicationId,
            string instanceId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ActivityInstance>(
                HttpMethod.Get,
                $"applications/{EscapeRouteValue(applicationId)}/activity-instances/{EscapeRouteValue(instanceId)}",
                $"applications/:application_id/activity-instances/:instance_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );
    }
}
