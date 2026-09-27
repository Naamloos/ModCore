using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Requests;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<
            RestResponse<ApplicationCommand[]>
        > GetGlobalApplicationCommandsWithOptionsAsync(
            Snowflake applicationId,
            GetGlobalApplicationCommandsQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ApplicationCommand[]>(
                HttpMethod.Get,
                $"applications/{EscapeRouteValue(applicationId)}/commands",
                $"applications/:application_id/commands",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<ApplicationCommand>> CreateGlobalApplicationCommandAsync(
            Snowflake applicationId,
            CreateGlobalApplicationCommandRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ApplicationCommand>(
                HttpMethod.Post,
                $"applications/{EscapeRouteValue(applicationId)}/commands",
                $"applications/:application_id/commands",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<ApplicationCommand>> GetGlobalApplicationCommandAsync(
            Snowflake applicationId,
            Snowflake commandId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ApplicationCommand>(
                HttpMethod.Get,
                $"applications/{EscapeRouteValue(applicationId)}/commands/{EscapeRouteValue(commandId)}",
                $"applications/:application_id/commands/:command_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<ApplicationCommand>> EditGlobalApplicationCommandAsync(
            Snowflake applicationId,
            Snowflake commandId,
            EditGlobalApplicationCommandRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ApplicationCommand>(
                HttpMethod.Patch,
                $"applications/{EscapeRouteValue(applicationId)}/commands/{EscapeRouteValue(commandId)}",
                $"applications/:application_id/commands/:command_id",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteGlobalApplicationCommandWithOptionsAsync(
            Snowflake applicationId,
            Snowflake commandId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"applications/{EscapeRouteValue(applicationId)}/commands/{EscapeRouteValue(commandId)}",
                $"applications/:application_id/commands/:command_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<
            RestResponse<ApplicationCommand[]>
        > BulkOverwriteGlobalApplicationCommandsWithOptionsAsync(
            Snowflake applicationId,
            CreateGlobalApplicationCommandRequest[] body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ApplicationCommand[]>(
                HttpMethod.Put,
                $"applications/{EscapeRouteValue(applicationId)}/commands",
                $"applications/:application_id/commands",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<ApplicationCommand[]>> GetGuildApplicationCommandsAsync(
            Snowflake applicationId,
            Snowflake guildId,
            GetGuildApplicationCommandsQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ApplicationCommand[]>(
                HttpMethod.Get,
                $"applications/{EscapeRouteValue(applicationId)}/guilds/{EscapeRouteValue(guildId)}/commands",
                $"applications/:application_id/guilds/{EscapeRouteValue(guildId)}/commands",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<ApplicationCommand>> CreateGuildApplicationCommandAsync(
            Snowflake applicationId,
            Snowflake guildId,
            CreateGuildApplicationCommandRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ApplicationCommand>(
                HttpMethod.Post,
                $"applications/{EscapeRouteValue(applicationId)}/guilds/{EscapeRouteValue(guildId)}/commands",
                $"applications/:application_id/guilds/{EscapeRouteValue(guildId)}/commands",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<ApplicationCommand>> GetGuildApplicationCommandAsync(
            Snowflake applicationId,
            Snowflake guildId,
            Snowflake commandId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ApplicationCommand>(
                HttpMethod.Get,
                $"applications/{EscapeRouteValue(applicationId)}/guilds/{EscapeRouteValue(guildId)}/commands/{EscapeRouteValue(commandId)}",
                $"applications/:application_id/guilds/{EscapeRouteValue(guildId)}/commands/:command_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<ApplicationCommand>> EditGuildApplicationCommandAsync(
            Snowflake applicationId,
            Snowflake guildId,
            Snowflake commandId,
            EditGuildApplicationCommandRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ApplicationCommand>(
                HttpMethod.Patch,
                $"applications/{EscapeRouteValue(applicationId)}/guilds/{EscapeRouteValue(guildId)}/commands/{EscapeRouteValue(commandId)}",
                $"applications/:application_id/guilds/{EscapeRouteValue(guildId)}/commands/:command_id",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteGuildApplicationCommandAsync(
            Snowflake applicationId,
            Snowflake guildId,
            Snowflake commandId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"applications/{EscapeRouteValue(applicationId)}/guilds/{EscapeRouteValue(guildId)}/commands/{EscapeRouteValue(commandId)}",
                $"applications/:application_id/guilds/{EscapeRouteValue(guildId)}/commands/:command_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<
            RestResponse<ApplicationCommand[]>
        > BulkOverwriteGuildApplicationCommandsAsync(
            Snowflake applicationId,
            Snowflake guildId,
            CreateGuildApplicationCommandRequest[] body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ApplicationCommand[]>(
                HttpMethod.Put,
                $"applications/{EscapeRouteValue(applicationId)}/guilds/{EscapeRouteValue(guildId)}/commands",
                $"applications/:application_id/guilds/{EscapeRouteValue(guildId)}/commands",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<
            RestResponse<GuildApplicationCommandPermissions[]>
        > GetGuildApplicationCommandPermissionsAsync(
            Snowflake applicationId,
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildApplicationCommandPermissions[]>(
                HttpMethod.Get,
                $"applications/{EscapeRouteValue(applicationId)}/guilds/{EscapeRouteValue(guildId)}/commands/permissions",
                $"applications/:application_id/guilds/{EscapeRouteValue(guildId)}/commands/permissions",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<
            RestResponse<GuildApplicationCommandPermissions>
        > GetApplicationCommandPermissionsAsync(
            Snowflake applicationId,
            Snowflake guildId,
            Snowflake commandId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildApplicationCommandPermissions>(
                HttpMethod.Get,
                $"applications/{EscapeRouteValue(applicationId)}/guilds/{EscapeRouteValue(guildId)}/commands/{EscapeRouteValue(commandId)}/permissions",
                $"applications/:application_id/guilds/{EscapeRouteValue(guildId)}/commands/:command_id/permissions",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<
            RestResponse<GuildApplicationCommandPermissions>
        > EditApplicationCommandPermissionsAsync(
            Snowflake applicationId,
            Snowflake guildId,
            Snowflake commandId,
            EditApplicationCommandPermissionsRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildApplicationCommandPermissions>(
                HttpMethod.Put,
                $"applications/{EscapeRouteValue(applicationId)}/guilds/{EscapeRouteValue(guildId)}/commands/{EscapeRouteValue(commandId)}/permissions",
                $"applications/:application_id/guilds/{EscapeRouteValue(guildId)}/commands/:command_id/permissions",
                body,
                null,
                auditLogReason,
                cancellationToken
            );
    }
}
