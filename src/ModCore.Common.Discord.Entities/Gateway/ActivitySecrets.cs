using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Gateway
{
    public record ActivitySecrets
    {
        [JsonPropertyName("join")]
        public Optional<string> Join { get; set; }

        [JsonPropertyName("spectate")]
        public Optional<string> Spectate { get; set; }

        [JsonPropertyName("match")]
        public Optional<string> Match { get; set; }
    }
}
