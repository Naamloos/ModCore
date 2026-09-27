using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Requests;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<GuildScheduledEvent[]>> ListScheduledEventsForGuildAsync(
            Snowflake guildId,
            ListScheduledEventsForGuildQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildScheduledEvent[]>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/scheduled-events",
                $"guilds/{EscapeRouteValue(guildId)}/scheduled-events",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<GuildScheduledEvent>> CreateGuildScheduledEventAsync(
            Snowflake guildId,
            CreateGuildScheduledEventRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildScheduledEvent>(
                HttpMethod.Post,
                $"guilds/{EscapeRouteValue(guildId)}/scheduled-events",
                $"guilds/{EscapeRouteValue(guildId)}/scheduled-events",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<GuildScheduledEvent>> GetGuildScheduledEventAsync(
            Snowflake guildId,
            Snowflake guildScheduledEventId,
            GetGuildScheduledEventQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildScheduledEvent>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/scheduled-events/{EscapeRouteValue(guildScheduledEventId)}",
                $"guilds/{EscapeRouteValue(guildId)}/scheduled-events/:guild_scheduled_event_id",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<GuildScheduledEvent>> ModifyGuildScheduledEventAsync(
            Snowflake guildId,
            Snowflake guildScheduledEventId,
            ModifyGuildScheduledEventRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildScheduledEvent>(
                HttpMethod.Patch,
                $"guilds/{EscapeRouteValue(guildId)}/scheduled-events/{EscapeRouteValue(guildScheduledEventId)}",
                $"guilds/{EscapeRouteValue(guildId)}/scheduled-events/:guild_scheduled_event_id",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteGuildScheduledEventAsync(
            Snowflake guildId,
            Snowflake guildScheduledEventId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"guilds/{EscapeRouteValue(guildId)}/scheduled-events/{EscapeRouteValue(guildScheduledEventId)}",
                $"guilds/{EscapeRouteValue(guildId)}/scheduled-events/:guild_scheduled_event_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<GuildScheduledEventUser[]>> GetGuildScheduledEventUsersAsync(
            Snowflake guildId,
            Snowflake guildScheduledEventId,
            GetGuildScheduledEventUsersQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildScheduledEventUser[]>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/scheduled-events/{EscapeRouteValue(guildScheduledEventId)}/users",
                $"guilds/{EscapeRouteValue(guildId)}/scheduled-events/:guild_scheduled_event_id/users",
                null,
                query,
                auditLogReason,
                cancellationToken
            );
    }
}
