using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Lobbies
{
    public record LobbyMember
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("metadata")]
        public Optional<Dictionary<string, string>?> Metadata { get; set; }

        [JsonPropertyName("flags")]
        public Optional<long> Flags { get; set; }

        [JsonPropertyName("additional_name")]
        public Optional<string> AdditionalName { get; set; }
    }
}
