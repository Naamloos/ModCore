using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Invites;
using ModCore.Common.Discord.Entities.Requests;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<Invite>> GetInviteAsync(
            string inviteCode,
            GetInviteQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Invite>(
                HttpMethod.Get,
                $"invites/{EscapeRouteValue(inviteCode)}",
                $"invites/:invite_code",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Invite>> DeleteInviteAsync(
            string inviteCode,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Invite>(
                HttpMethod.Delete,
                $"invites/{EscapeRouteValue(inviteCode)}",
                $"invites/:invite_code",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<string>> GetTargetUsersAsync(
            string inviteCode,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<string>(
                HttpMethod.Get,
                $"invites/{EscapeRouteValue(inviteCode)}/target-users",
                $"invites/:invite_code/target-users",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> UpdateTargetUsersAsync(
            string inviteCode,
            MultipartRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Put,
                $"invites/{EscapeRouteValue(inviteCode)}/target-users",
                $"invites/:invite_code/target-users",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<TargetUsersJobStatus>> GetTargetUsersJobStatusAsync(
            string inviteCode,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<TargetUsersJobStatus>(
                HttpMethod.Get,
                $"invites/{EscapeRouteValue(inviteCode)}/target-users/job-status",
                $"invites/:invite_code/target-users/job-status",
                null,
                null,
                auditLogReason,
                cancellationToken
            );
    }
}
