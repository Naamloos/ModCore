using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Lobbies;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record CreateLobbyRequest
    {
        [JsonPropertyName("metadata")]
        public Optional<Dictionary<string, string>?> Metadata { get; set; }

        [JsonPropertyName("members")]
        public Optional<LobbyMember[]> Members { get; set; }

        [JsonPropertyName("idle_timeout_seconds")]
        public Optional<int> IdleTimeoutSeconds { get; set; }
    }

    public record CreateLobbyLobbyMemberJSONRequest
    {
        [JsonPropertyName("id")]
        public Optional<Snowflake> Id { get; set; }

        [JsonPropertyName("metadata")]
        public Optional<Dictionary<string, string>?> Metadata { get; set; }

        [JsonPropertyName("flags")]
        public Optional<long> Flags { get; set; }

        [JsonPropertyName("additional_name")]
        public Optional<string?> AdditionalName { get; set; }
    }

    public record CreateOrJoinLobbyRequest
    {
        [JsonPropertyName("secret")]
        public Optional<string> Secret { get; set; }

        [JsonPropertyName("idle_timeout_seconds")]
        public Optional<int> IdleTimeoutSeconds { get; set; }

        [JsonPropertyName("lobby_metadata")]
        public Optional<Dictionary<string, string>?> LobbyMetadata { get; set; }

        [JsonPropertyName("member_metadata")]
        public Optional<Dictionary<string, string>?> MemberMetadata { get; set; }
    }

    public record ModifyLobbyRequest
    {
        [JsonPropertyName("metadata")]
        public Optional<Dictionary<string, string>?> Metadata { get; set; }

        [JsonPropertyName("members")]
        public Optional<LobbyMember[]> Members { get; set; }

        [JsonPropertyName("idle_timeout_seconds")]
        public Optional<int> IdleTimeoutSeconds { get; set; }
    }

    public record AddAMemberToALobbyRequest
    {
        [JsonPropertyName("metadata")]
        public Optional<Dictionary<string, string>?> Metadata { get; set; }

        [JsonPropertyName("flags")]
        public Optional<long> Flags { get; set; }

        [JsonPropertyName("additional_name")]
        public Optional<string?> AdditionalName { get; set; }
    }

    public record BulkUpdateLobbyMembersRequest
    {
        [JsonPropertyName("id")]
        public Optional<Snowflake> Id { get; set; }

        [JsonPropertyName("metadata")]
        public Optional<Dictionary<string, string>?> Metadata { get; set; }

        [JsonPropertyName("flags")]
        public Optional<long> Flags { get; set; }

        [JsonPropertyName("additional_name")]
        public Optional<string?> AdditionalName { get; set; }

        [JsonPropertyName("remove_member")]
        public Optional<bool> RemoveMember { get; set; }
    }

    public record LinkChannelToLobbyRequest
    {
        [JsonPropertyName("channel_id")]
        public Optional<Snowflake> ChannelId { get; set; }
    }

    public record SendLobbyMessageRequest
    {
        [JsonPropertyName("content")]
        public Optional<string> Content { get; set; }

        [JsonPropertyName("metadata")]
        public Optional<Dictionary<string, string>?> Metadata { get; set; }

        [JsonPropertyName("flags")]
        public Optional<long> Flags { get; set; }
    }

    public record GetLobbyMessagesQuery
    {
        [JsonPropertyName("limit")]
        public Optional<int> Limit { get; set; }
    }
}
