using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Gateway
{
    public record ActivityParty
    {
        [JsonPropertyName("id")]
        public Optional<string> Id { get; set; }

        [JsonPropertyName("size")]
        public Optional<int[]> Size { get; set; }
    }
}
