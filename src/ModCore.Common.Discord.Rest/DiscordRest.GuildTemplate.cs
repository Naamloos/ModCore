using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Requests;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<GuildTemplate>> GetGuildTemplateAsync(
            string templateCode,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildTemplate>(
                HttpMethod.Get,
                $"guilds/templates/{EscapeRouteValue(templateCode)}",
                $"guilds/templates/:template_code",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<GuildTemplate[]>> GetGuildTemplatesAsync(
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildTemplate[]>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/templates",
                $"guilds/{EscapeRouteValue(guildId)}/templates",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<GuildTemplate>> CreateGuildTemplateAsync(
            Snowflake guildId,
            CreateGuildTemplateRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildTemplate>(
                HttpMethod.Post,
                $"guilds/{EscapeRouteValue(guildId)}/templates",
                $"guilds/{EscapeRouteValue(guildId)}/templates",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<GuildTemplate>> SyncGuildTemplateAsync(
            Snowflake guildId,
            string templateCode,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildTemplate>(
                HttpMethod.Put,
                $"guilds/{EscapeRouteValue(guildId)}/templates/{EscapeRouteValue(templateCode)}",
                $"guilds/{EscapeRouteValue(guildId)}/templates/:template_code",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<GuildTemplate>> ModifyGuildTemplateAsync(
            Snowflake guildId,
            string templateCode,
            ModifyGuildTemplateRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildTemplate>(
                HttpMethod.Patch,
                $"guilds/{EscapeRouteValue(guildId)}/templates/{EscapeRouteValue(templateCode)}",
                $"guilds/{EscapeRouteValue(guildId)}/templates/:template_code",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<GuildTemplate>> DeleteGuildTemplateAsync(
            Snowflake guildId,
            string templateCode,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildTemplate>(
                HttpMethod.Delete,
                $"guilds/{EscapeRouteValue(guildId)}/templates/{EscapeRouteValue(templateCode)}",
                $"guilds/{EscapeRouteValue(guildId)}/templates/:template_code",
                null,
                null,
                auditLogReason,
                cancellationToken
            );
    }
}
