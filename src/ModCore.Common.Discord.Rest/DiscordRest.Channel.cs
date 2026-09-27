using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Channels;
using ModCore.Common.Discord.Entities.Invites;
using ModCore.Common.Discord.Entities.Requests;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<Channel>> GetChannelAsync(
            Snowflake channelId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Channel>(
                HttpMethod.Get,
                $"channels/{EscapeRouteValue(channelId)}",
                $"channels/{EscapeRouteValue(channelId)}",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Channel>> ModifyChannelAsync(
            Snowflake channelId,
            ModifyChannelRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Channel>(
                HttpMethod.Patch,
                $"channels/{EscapeRouteValue(channelId)}",
                $"channels/{EscapeRouteValue(channelId)}",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> SetVoiceChannelStatusAsync(
            Snowflake channelId,
            SetVoiceChannelStatusRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Put,
                $"channels/{EscapeRouteValue(channelId)}/voice-status",
                $"channels/{EscapeRouteValue(channelId)}/voice-status",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Channel>> DeleteCloseChannelAsync(
            Snowflake channelId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Channel>(
                HttpMethod.Delete,
                $"channels/{EscapeRouteValue(channelId)}",
                $"channels/{EscapeRouteValue(channelId)}",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> EditChannelPermissionsAsync(
            Snowflake channelId,
            Snowflake overwriteId,
            EditChannelPermissionsRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Put,
                $"channels/{EscapeRouteValue(channelId)}/permissions/{EscapeRouteValue(overwriteId)}",
                $"channels/{EscapeRouteValue(channelId)}/permissions/:overwrite_id",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Invite[]>> GetChannelInvitesAsync(
            Snowflake channelId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Invite[]>(
                HttpMethod.Get,
                $"channels/{EscapeRouteValue(channelId)}/invites",
                $"channels/{EscapeRouteValue(channelId)}/invites",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Invite>> CreateChannelInviteAsync(
            Snowflake channelId,
            CreateChannelInviteRequest body,
            IReadOnlyList<UploadFile>? files = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Invite>(
                HttpMethod.Post,
                $"channels/{EscapeRouteValue(channelId)}/invites",
                $"channels/{EscapeRouteValue(channelId)}/invites",
                files == null ? body : new MultipartRequest { Payload = body, Files = files },
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteChannelPermissionAsync(
            Snowflake channelId,
            Snowflake overwriteId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"channels/{EscapeRouteValue(channelId)}/permissions/{EscapeRouteValue(overwriteId)}",
                $"channels/{EscapeRouteValue(channelId)}/permissions/:overwrite_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<FollowedChannel>> FollowAnnouncementChannelAsync(
            Snowflake channelId,
            FollowAnnouncementChannelRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<FollowedChannel>(
                HttpMethod.Post,
                $"channels/{EscapeRouteValue(channelId)}/followers",
                $"channels/{EscapeRouteValue(channelId)}/followers",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> TriggerTypingIndicatorAsync(
            Snowflake channelId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Post,
                $"channels/{EscapeRouteValue(channelId)}/typing",
                $"channels/{EscapeRouteValue(channelId)}/typing",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> GroupDMAddRecipientAsync(
            Snowflake channelId,
            Snowflake userId,
            GroupDMAddRecipientRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Put,
                $"channels/{EscapeRouteValue(channelId)}/recipients/{EscapeRouteValue(userId)}",
                $"channels/{EscapeRouteValue(channelId)}/recipients/:user_id",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> GroupDMRemoveRecipientAsync(
            Snowflake channelId,
            Snowflake userId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"channels/{EscapeRouteValue(channelId)}/recipients/{EscapeRouteValue(userId)}",
                $"channels/{EscapeRouteValue(channelId)}/recipients/:user_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Channel>> StartThreadFromMessageAsync(
            Snowflake channelId,
            Snowflake messageId,
            StartThreadFromMessageRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Channel>(
                HttpMethod.Post,
                $"channels/{EscapeRouteValue(channelId)}/messages/{EscapeRouteValue(messageId)}/threads",
                $"channels/{EscapeRouteValue(channelId)}/messages/:message_id/threads",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Channel>> StartThreadWithoutMessageAsync(
            Snowflake channelId,
            StartThreadWithoutMessageRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Channel>(
                HttpMethod.Post,
                $"channels/{EscapeRouteValue(channelId)}/threads",
                $"channels/{EscapeRouteValue(channelId)}/threads",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Channel>> StartThreadInForumOrMediaChannelAsync(
            Snowflake channelId,
            StartThreadInForumOrMediaChannelRequest body,
            IReadOnlyList<UploadFile>? files = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Channel>(
                HttpMethod.Post,
                $"channels/{EscapeRouteValue(channelId)}/threads",
                $"channels/{EscapeRouteValue(channelId)}/threads",
                files == null ? body : new MultipartRequest { Payload = body, Files = files },
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> JoinThreadAsync(
            Snowflake channelId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Put,
                $"channels/{EscapeRouteValue(channelId)}/thread-members/@me",
                $"channels/{EscapeRouteValue(channelId)}/thread-members/@me",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> AddThreadMemberAsync(
            Snowflake channelId,
            Snowflake userId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Put,
                $"channels/{EscapeRouteValue(channelId)}/thread-members/{EscapeRouteValue(userId)}",
                $"channels/{EscapeRouteValue(channelId)}/thread-members/:user_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> LeaveThreadAsync(
            Snowflake channelId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"channels/{EscapeRouteValue(channelId)}/thread-members/@me",
                $"channels/{EscapeRouteValue(channelId)}/thread-members/@me",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> RemoveThreadMemberAsync(
            Snowflake channelId,
            Snowflake userId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"channels/{EscapeRouteValue(channelId)}/thread-members/{EscapeRouteValue(userId)}",
                $"channels/{EscapeRouteValue(channelId)}/thread-members/:user_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<ThreadMember>> GetThreadMemberAsync(
            Snowflake channelId,
            Snowflake userId,
            GetThreadMemberQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ThreadMember>(
                HttpMethod.Get,
                $"channels/{EscapeRouteValue(channelId)}/thread-members/{EscapeRouteValue(userId)}",
                $"channels/{EscapeRouteValue(channelId)}/thread-members/:user_id",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<ThreadMember[]>> ListThreadMembersAsync(
            Snowflake channelId,
            ListThreadMembersQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ThreadMember[]>(
                HttpMethod.Get,
                $"channels/{EscapeRouteValue(channelId)}/thread-members",
                $"channels/{EscapeRouteValue(channelId)}/thread-members",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<ArchivedThreads>> ListPublicArchivedThreadsAsync(
            Snowflake channelId,
            ListPublicArchivedThreadsQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ArchivedThreads>(
                HttpMethod.Get,
                $"channels/{EscapeRouteValue(channelId)}/threads/archived/public",
                $"channels/{EscapeRouteValue(channelId)}/threads/archived/public",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<ArchivedThreads>> ListPrivateArchivedThreadsAsync(
            Snowflake channelId,
            ListPrivateArchivedThreadsQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ArchivedThreads>(
                HttpMethod.Get,
                $"channels/{EscapeRouteValue(channelId)}/threads/archived/private",
                $"channels/{EscapeRouteValue(channelId)}/threads/archived/private",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<ArchivedThreads>> ListJoinedPrivateArchivedThreadsAsync(
            Snowflake channelId,
            ListJoinedPrivateArchivedThreadsQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ArchivedThreads>(
                HttpMethod.Get,
                $"channels/{EscapeRouteValue(channelId)}/users/@me/threads/archived/private",
                $"channels/{EscapeRouteValue(channelId)}/users/@me/threads/archived/private",
                null,
                query,
                auditLogReason,
                cancellationToken
            );
    }
}
