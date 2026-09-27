using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Requests;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<AuditLog>> GetGuildAuditLogAsync(
            Snowflake guildId,
            GetGuildAuditLogQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<AuditLog>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/audit-logs",
                $"guilds/{EscapeRouteValue(guildId)}/audit-logs",
                null,
                query,
                auditLogReason,
                cancellationToken
            );
    }
}
