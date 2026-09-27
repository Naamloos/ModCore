using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Lobbies;
using ModCore.Common.Discord.Entities.Requests;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<Lobby>> CreateLobbyAsync(
            CreateLobbyRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Lobby>(
                HttpMethod.Post,
                $"lobbies",
                $"lobbies",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Lobby>> CreateOrJoinLobbyAsync(
            CreateOrJoinLobbyRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Lobby>(
                HttpMethod.Put,
                $"lobbies",
                $"lobbies",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Lobby>> GetLobbyAsync(
            Snowflake lobbyId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Lobby>(
                HttpMethod.Get,
                $"lobbies/{EscapeRouteValue(lobbyId)}",
                $"lobbies/:lobby_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Lobby>> ModifyLobbyAsync(
            Snowflake lobbyId,
            ModifyLobbyRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Lobby>(
                HttpMethod.Patch,
                $"lobbies/{EscapeRouteValue(lobbyId)}",
                $"lobbies/:lobby_id",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteLobbyAsync(
            Snowflake lobbyId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"lobbies/{EscapeRouteValue(lobbyId)}",
                $"lobbies/:lobby_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<LobbyMember>> AddAMemberToALobbyAsync(
            Snowflake lobbyId,
            Snowflake userId,
            AddAMemberToALobbyRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<LobbyMember>(
                HttpMethod.Put,
                $"lobbies/{EscapeRouteValue(lobbyId)}/members/{EscapeRouteValue(userId)}",
                $"lobbies/:lobby_id/members/:user_id",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<LobbyMember[]>> BulkUpdateLobbyMembersAsync(
            Snowflake lobbyId,
            BulkUpdateLobbyMembersRequest[] body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<LobbyMember[]>(
                HttpMethod.Post,
                $"lobbies/{EscapeRouteValue(lobbyId)}/members/bulk",
                $"lobbies/:lobby_id/members/bulk",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> RemoveAMemberFromALobbyAsync(
            Snowflake lobbyId,
            Snowflake userId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"lobbies/{EscapeRouteValue(lobbyId)}/members/{EscapeRouteValue(userId)}",
                $"lobbies/:lobby_id/members/:user_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> LeaveLobbyAsync(
            Snowflake lobbyId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"lobbies/{EscapeRouteValue(lobbyId)}/members/@me",
                $"lobbies/:lobby_id/members/@me",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Lobby>> LinkChannelToLobbyAsync(
            Snowflake lobbyId,
            LinkChannelToLobbyRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Lobby>(
                HttpMethod.Patch,
                $"lobbies/{EscapeRouteValue(lobbyId)}/channel-linking",
                $"lobbies/:lobby_id/channel-linking",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Lobby>> UnlinkChannelFromLobbyAsync(
            Snowflake lobbyId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Lobby>(
                HttpMethod.Patch,
                $"lobbies/{EscapeRouteValue(lobbyId)}/channel-linking",
                $"lobbies/:lobby_id/channel-linking",
                new { channel_id = (string?)null },
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<LobbyMessage>> SendLobbyMessageAsync(
            Snowflake lobbyId,
            SendLobbyMessageRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<LobbyMessage>(
                HttpMethod.Post,
                $"lobbies/{EscapeRouteValue(lobbyId)}/messages",
                $"lobbies/:lobby_id/messages",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<LobbyMessage[]>> GetLobbyMessagesAsync(
            Snowflake lobbyId,
            GetLobbyMessagesQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<LobbyMessage[]>(
                HttpMethod.Get,
                $"lobbies/{EscapeRouteValue(lobbyId)}/messages",
                $"lobbies/:lobby_id/messages",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> UpdateLobbyMessageModerationMetadataAsync(
            Snowflake lobbyId,
            Snowflake messageId,
            Dictionary<string, string> body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Put,
                $"lobbies/{EscapeRouteValue(lobbyId)}/messages/{EscapeRouteValue(messageId)}/moderation-metadata",
                $"lobbies/:lobby_id/messages/:message_id/moderation-metadata",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<LobbyInvite>> CreateLobbyChannelInviteForSelfAsync(
            Snowflake lobbyId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<LobbyInvite>(
                HttpMethod.Post,
                $"lobbies/{EscapeRouteValue(lobbyId)}/members/@me/invites",
                $"lobbies/:lobby_id/members/@me/invites",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<LobbyInvite>> CreateLobbyChannelInviteForUserAsync(
            Snowflake lobbyId,
            Snowflake userId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<LobbyInvite>(
                HttpMethod.Post,
                $"lobbies/{EscapeRouteValue(lobbyId)}/members/{EscapeRouteValue(userId)}/invites",
                $"lobbies/:lobby_id/members/:user_id/invites",
                null,
                null,
                auditLogReason,
                cancellationToken
            );
    }
}
