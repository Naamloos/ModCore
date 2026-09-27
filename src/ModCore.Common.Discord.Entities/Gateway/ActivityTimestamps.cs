using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Gateway
{
    public record ActivityTimestamps
    {
        [JsonPropertyName("start")]
        public Optional<long> Start { get; set; }

        [JsonPropertyName("end")]
        public Optional<long> End { get; set; }
    }
}
