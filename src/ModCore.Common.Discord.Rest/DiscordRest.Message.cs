using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.Discord.Entities.Requests;
using ModCore.Common.Discord.Entities.Responses;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<Message[]>> GetChannelMessagesAsync(
            Snowflake channelId,
            GetChannelMessagesQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Message[]>(
                HttpMethod.Get,
                $"channels/{EscapeRouteValue(channelId)}/messages",
                $"channels/{EscapeRouteValue(channelId)}/messages",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<SearchGuildMessagesResponse>> SearchGuildMessagesAsync(
            Snowflake guildId,
            SearchGuildMessagesQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<SearchGuildMessagesResponse>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/messages/search",
                $"guilds/{EscapeRouteValue(guildId)}/messages/search",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Message>> GetChannelMessageAsync(
            Snowflake channelId,
            Snowflake messageId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Message>(
                HttpMethod.Get,
                $"channels/{EscapeRouteValue(channelId)}/messages/{EscapeRouteValue(messageId)}",
                $"channels/{EscapeRouteValue(channelId)}/messages/:message_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Message>> CreateMessageWithOptionsAsync(
            Snowflake channelId,
            CreateMessageRequest body,
            IReadOnlyList<UploadFile>? files = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Message>(
                HttpMethod.Post,
                $"channels/{EscapeRouteValue(channelId)}/messages",
                $"channels/{EscapeRouteValue(channelId)}/messages",
                files == null ? body : new MultipartRequest { Payload = body, Files = files },
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Message>> CrosspostMessageAsync(
            Snowflake channelId,
            Snowflake messageId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Message>(
                HttpMethod.Post,
                $"channels/{EscapeRouteValue(channelId)}/messages/{EscapeRouteValue(messageId)}/crosspost",
                $"channels/{EscapeRouteValue(channelId)}/messages/:message_id/crosspost",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> CreateReactionAsync(
            Snowflake channelId,
            Snowflake messageId,
            string emojiId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Put,
                $"channels/{EscapeRouteValue(channelId)}/messages/{EscapeRouteValue(messageId)}/reactions/{EscapeRouteValue(emojiId)}/@me",
                $"channels/{EscapeRouteValue(channelId)}/messages/:message_id/reactions/:emoji_id/@me",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteOwnReactionAsync(
            Snowflake channelId,
            Snowflake messageId,
            string emojiId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"channels/{EscapeRouteValue(channelId)}/messages/{EscapeRouteValue(messageId)}/reactions/{EscapeRouteValue(emojiId)}/@me",
                $"channels/{EscapeRouteValue(channelId)}/messages/:message_id/reactions/:emoji_id/@me",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteUserReactionAsync(
            Snowflake channelId,
            Snowflake messageId,
            string emojiId,
            Snowflake userId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"channels/{EscapeRouteValue(channelId)}/messages/{EscapeRouteValue(messageId)}/reactions/{EscapeRouteValue(emojiId)}/{EscapeRouteValue(userId)}",
                $"channels/{EscapeRouteValue(channelId)}/messages/:message_id/reactions/:emoji_id/:user_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<User[]>> GetReactionsAsync(
            Snowflake channelId,
            Snowflake messageId,
            string emojiId,
            GetReactionsQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<User[]>(
                HttpMethod.Get,
                $"channels/{EscapeRouteValue(channelId)}/messages/{EscapeRouteValue(messageId)}/reactions/{EscapeRouteValue(emojiId)}",
                $"channels/{EscapeRouteValue(channelId)}/messages/:message_id/reactions/:emoji_id",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteAllReactionsAsync(
            Snowflake channelId,
            Snowflake messageId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"channels/{EscapeRouteValue(channelId)}/messages/{EscapeRouteValue(messageId)}/reactions",
                $"channels/{EscapeRouteValue(channelId)}/messages/:message_id/reactions",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteAllReactionsForEmojiAsync(
            Snowflake channelId,
            Snowflake messageId,
            string emojiId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"channels/{EscapeRouteValue(channelId)}/messages/{EscapeRouteValue(messageId)}/reactions/{EscapeRouteValue(emojiId)}",
                $"channels/{EscapeRouteValue(channelId)}/messages/:message_id/reactions/:emoji_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Message>> EditMessageAsync(
            Snowflake channelId,
            Snowflake messageId,
            EditMessageRequest body,
            IReadOnlyList<UploadFile>? files = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Message>(
                HttpMethod.Patch,
                $"channels/{EscapeRouteValue(channelId)}/messages/{EscapeRouteValue(messageId)}",
                $"channels/{EscapeRouteValue(channelId)}/messages/:message_id",
                files == null ? body : new MultipartRequest { Payload = body, Files = files },
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteMessageAsync(
            Snowflake channelId,
            Snowflake messageId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"channels/{EscapeRouteValue(channelId)}/messages/{EscapeRouteValue(messageId)}",
                $"channels/{EscapeRouteValue(channelId)}/messages/:message_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> BulkDeleteMessagesAsync(
            Snowflake channelId,
            BulkDeleteMessagesRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Post,
                $"channels/{EscapeRouteValue(channelId)}/messages/bulk-delete",
                $"channels/{EscapeRouteValue(channelId)}/messages/bulk-delete",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<GetChannelPinsResponse>> GetChannelPinsAsync(
            Snowflake channelId,
            GetChannelPinsQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GetChannelPinsResponse>(
                HttpMethod.Get,
                $"channels/{EscapeRouteValue(channelId)}/messages/pins",
                $"channels/{EscapeRouteValue(channelId)}/messages/pins",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> PinMessageAsync(
            Snowflake channelId,
            Snowflake messageId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Put,
                $"channels/{EscapeRouteValue(channelId)}/messages/pins/{EscapeRouteValue(messageId)}",
                $"channels/{EscapeRouteValue(channelId)}/messages/pins/:message_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> UnpinMessageAsync(
            Snowflake channelId,
            Snowflake messageId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"channels/{EscapeRouteValue(channelId)}/messages/pins/{EscapeRouteValue(messageId)}",
                $"channels/{EscapeRouteValue(channelId)}/messages/pins/:message_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Message[]>> GetPinnedMessagesLegacyAsync(
            Snowflake channelId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Message[]>(
                HttpMethod.Get,
                $"channels/{EscapeRouteValue(channelId)}/pins",
                $"channels/{EscapeRouteValue(channelId)}/pins",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> PinMessageLegacyAsync(
            Snowflake channelId,
            Snowflake messageId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Put,
                $"channels/{EscapeRouteValue(channelId)}/pins/{EscapeRouteValue(messageId)}",
                $"channels/{EscapeRouteValue(channelId)}/pins/:message_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> UnpinMessageLegacyAsync(
            Snowflake channelId,
            Snowflake messageId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"channels/{EscapeRouteValue(channelId)}/pins/{EscapeRouteValue(messageId)}",
                $"channels/{EscapeRouteValue(channelId)}/pins/:message_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );
    }
}
