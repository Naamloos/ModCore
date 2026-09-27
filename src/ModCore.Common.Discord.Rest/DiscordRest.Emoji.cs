using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Requests;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<Emoji[]>> ListGuildEmojisAsync(
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Emoji[]>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/emojis",
                $"guilds/{EscapeRouteValue(guildId)}/emojis",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Emoji>> GetGuildEmojiAsync(
            Snowflake guildId,
            Snowflake emojiId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Emoji>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/emojis/{EscapeRouteValue(emojiId)}",
                $"guilds/{EscapeRouteValue(guildId)}/emojis/:emoji_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Emoji>> CreateGuildEmojiAsync(
            Snowflake guildId,
            CreateGuildEmojiRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Emoji>(
                HttpMethod.Post,
                $"guilds/{EscapeRouteValue(guildId)}/emojis",
                $"guilds/{EscapeRouteValue(guildId)}/emojis",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Emoji>> ModifyGuildEmojiAsync(
            Snowflake guildId,
            Snowflake emojiId,
            ModifyGuildEmojiRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Emoji>(
                HttpMethod.Patch,
                $"guilds/{EscapeRouteValue(guildId)}/emojis/{EscapeRouteValue(emojiId)}",
                $"guilds/{EscapeRouteValue(guildId)}/emojis/:emoji_id",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteGuildEmojiAsync(
            Snowflake guildId,
            Snowflake emojiId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"guilds/{EscapeRouteValue(guildId)}/emojis/{EscapeRouteValue(emojiId)}",
                $"guilds/{EscapeRouteValue(guildId)}/emojis/:emoji_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<ApplicationEmojis>> ListApplicationEmojisAsync(
            Snowflake applicationId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ApplicationEmojis>(
                HttpMethod.Get,
                $"applications/{EscapeRouteValue(applicationId)}/emojis",
                $"applications/:application_id/emojis",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Emoji>> GetApplicationEmojiAsync(
            Snowflake applicationId,
            Snowflake emojiId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Emoji>(
                HttpMethod.Get,
                $"applications/{EscapeRouteValue(applicationId)}/emojis/{EscapeRouteValue(emojiId)}",
                $"applications/:application_id/emojis/:emoji_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Emoji>> CreateApplicationEmojiAsync(
            Snowflake applicationId,
            CreateApplicationEmojiRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Emoji>(
                HttpMethod.Post,
                $"applications/{EscapeRouteValue(applicationId)}/emojis",
                $"applications/:application_id/emojis",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Emoji>> ModifyApplicationEmojiAsync(
            Snowflake applicationId,
            Snowflake emojiId,
            ModifyApplicationEmojiRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Emoji>(
                HttpMethod.Patch,
                $"applications/{EscapeRouteValue(applicationId)}/emojis/{EscapeRouteValue(emojiId)}",
                $"applications/:application_id/emojis/:emoji_id",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteApplicationEmojiAsync(
            Snowflake applicationId,
            Snowflake emojiId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"applications/{EscapeRouteValue(applicationId)}/emojis/{EscapeRouteValue(emojiId)}",
                $"applications/:application_id/emojis/:emoji_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );
    }
}
