using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Requests;
using ModCore.Common.Discord.Entities.Voice;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<object>> SendSoundboardSoundAsync(
            Snowflake channelId,
            SendSoundboardSoundRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Post,
                $"channels/{EscapeRouteValue(channelId)}/send-soundboard-sound",
                $"channels/{EscapeRouteValue(channelId)}/send-soundboard-sound",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<SoundboardSound[]>> ListDefaultSoundboardSoundsAsync(
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<SoundboardSound[]>(
                HttpMethod.Get,
                $"soundboard-default-sounds",
                $"soundboard-default-sounds",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<GuildSoundboardSounds>> ListGuildSoundboardSoundsAsync(
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildSoundboardSounds>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/soundboard-sounds",
                $"guilds/{EscapeRouteValue(guildId)}/soundboard-sounds",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<SoundboardSound>> GetGuildSoundboardSoundAsync(
            Snowflake guildId,
            Snowflake soundId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<SoundboardSound>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/soundboard-sounds/{EscapeRouteValue(soundId)}",
                $"guilds/{EscapeRouteValue(guildId)}/soundboard-sounds/:sound_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<SoundboardSound>> CreateGuildSoundboardSoundAsync(
            Snowflake guildId,
            CreateGuildSoundboardSoundRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<SoundboardSound>(
                HttpMethod.Post,
                $"guilds/{EscapeRouteValue(guildId)}/soundboard-sounds",
                $"guilds/{EscapeRouteValue(guildId)}/soundboard-sounds",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<SoundboardSound>> ModifyGuildSoundboardSoundAsync(
            Snowflake guildId,
            Snowflake soundId,
            ModifyGuildSoundboardSoundRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<SoundboardSound>(
                HttpMethod.Patch,
                $"guilds/{EscapeRouteValue(guildId)}/soundboard-sounds/{EscapeRouteValue(soundId)}",
                $"guilds/{EscapeRouteValue(guildId)}/soundboard-sounds/:sound_id",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteGuildSoundboardSoundAsync(
            Snowflake guildId,
            Snowflake soundId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"guilds/{EscapeRouteValue(guildId)}/soundboard-sounds/{EscapeRouteValue(soundId)}",
                $"guilds/{EscapeRouteValue(guildId)}/soundboard-sounds/:sound_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );
    }
}
