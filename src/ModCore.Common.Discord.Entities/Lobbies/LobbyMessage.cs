using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Lobbies
{
    public record LobbyMessage
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; } = default!;

        [JsonPropertyName("lobby_id")]
        public Snowflake LobbyId { get; set; }

        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; set; }

        [JsonPropertyName("author")]
        public User Author { get; set; }

        [JsonPropertyName("lobby_member")]
        public Optional<LobbyMember> LobbyMember { get; set; }

        [JsonPropertyName("metadata")]
        public Optional<Dictionary<string, string>?> Metadata { get; set; }

        [JsonPropertyName("moderation_metadata")]
        public Optional<Dictionary<string, string>?> ModerationMetadata { get; set; }

        [JsonPropertyName("flags")]
        public long Flags { get; set; }

        [JsonPropertyName("application_id")]
        public Snowflake ApplicationId { get; set; }
    }
}
