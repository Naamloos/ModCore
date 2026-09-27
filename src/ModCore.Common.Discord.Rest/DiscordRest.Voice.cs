using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Requests;
using ModCore.Common.Discord.Entities.Voice;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<VoiceRegion[]>> ListVoiceRegionsAsync(
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<VoiceRegion[]>(
                HttpMethod.Get,
                $"voice/regions",
                $"voice/regions",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<VoiceState>> GetCurrentUserVoiceStateAsync(
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<VoiceState>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/voice-states/@me",
                $"guilds/{EscapeRouteValue(guildId)}/voice-states/@me",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<VoiceState>> GetUserVoiceStateAsync(
            Snowflake guildId,
            Snowflake userId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<VoiceState>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/voice-states/{EscapeRouteValue(userId)}",
                $"guilds/{EscapeRouteValue(guildId)}/voice-states/:user_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> ModifyCurrentUserVoiceStateAsync(
            Snowflake guildId,
            ModifyCurrentUserVoiceStateRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Patch,
                $"guilds/{EscapeRouteValue(guildId)}/voice-states/@me",
                $"guilds/{EscapeRouteValue(guildId)}/voice-states/@me",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> ModifyUserVoiceStateAsync(
            Snowflake guildId,
            Snowflake userId,
            ModifyUserVoiceStateRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Patch,
                $"guilds/{EscapeRouteValue(guildId)}/voice-states/{EscapeRouteValue(userId)}",
                $"guilds/{EscapeRouteValue(guildId)}/voice-states/:user_id",
                body,
                null,
                auditLogReason,
                cancellationToken
            );
    }
}
