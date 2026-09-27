using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Channels;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Requests;
using ModCore.Common.Discord.Entities.Users;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<User>> GetCurrentUserWithOptionsAsync(
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<User>(
                HttpMethod.Get,
                $"users/@me",
                $"users/@me",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<User>> GetUserWithOptionsAsync(
            Snowflake userId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<User>(
                HttpMethod.Get,
                $"users/{EscapeRouteValue(userId)}",
                $"users/:user_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<User>> ModifyCurrentUserAsync(
            ModifyCurrentUserRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<User>(
                HttpMethod.Patch,
                $"users/@me",
                $"users/@me",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<List<CurrentUserGuild>>> GetCurrentUserGuildsAsync(
            GetCurrentUserGuildsQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<List<CurrentUserGuild>>(
                HttpMethod.Get,
                $"users/@me/guilds",
                $"users/@me/guilds",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Member>> GetCurrentUserGuildMemberAsync(
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Member>(
                HttpMethod.Get,
                $"users/@me/guilds/{EscapeRouteValue(guildId)}/member",
                $"users/@me/guilds/{EscapeRouteValue(guildId)}/member",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> LeaveGuildAsync(
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"users/@me/guilds/{EscapeRouteValue(guildId)}",
                $"users/@me/guilds/{EscapeRouteValue(guildId)}",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Channel>> CreateDMAsync(
            CreateDMRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Channel>(
                HttpMethod.Post,
                $"users/@me/channels",
                $"users/@me/channels",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Channel>> CreateGroupDMAsync(
            CreateGroupDMRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Channel>(
                HttpMethod.Post,
                $"users/@me/channels",
                $"users/@me/channels",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Connection[]>> GetCurrentUserConnectionsAsync(
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Connection[]>(
                HttpMethod.Get,
                $"users/@me/connections",
                $"users/@me/connections",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<
            RestResponse<ApplicationRoleConnection>
        > GetCurrentUserApplicationRoleConnectionAsync(
            Snowflake applicationId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ApplicationRoleConnection>(
                HttpMethod.Get,
                $"users/@me/applications/{EscapeRouteValue(applicationId)}/role-connection",
                $"users/@me/applications/:application_id/role-connection",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<
            RestResponse<ApplicationRoleConnection>
        > UpdateCurrentUserApplicationRoleConnectionAsync(
            Snowflake applicationId,
            UpdateCurrentUserApplicationRoleConnectionRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ApplicationRoleConnection>(
                HttpMethod.Put,
                $"users/@me/applications/{EscapeRouteValue(applicationId)}/role-connection",
                $"users/@me/applications/:application_id/role-connection",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteCurrentUserApplicationRoleConnectionAsync(
            Snowflake applicationId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"users/@me/applications/{EscapeRouteValue(applicationId)}/role-connection",
                $"users/@me/applications/:application_id/role-connection",
                null,
                null,
                auditLogReason,
                cancellationToken
            );
    }
}
