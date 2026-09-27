using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Channels;

namespace ModCore.Common.Discord.Entities.Lobbies
{
    public record Lobby
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("application_id")]
        public Snowflake ApplicationId { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }

        [JsonPropertyName("members")]
        public LobbyMember[] Members { get; set; } = default!;

        [JsonPropertyName("linked_channel")]
        public Optional<Channel> LinkedChannel { get; set; }
    }
}
