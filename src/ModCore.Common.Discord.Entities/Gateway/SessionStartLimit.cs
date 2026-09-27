using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Gateway
{
    public record SessionStartLimit
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("remaining")]
        public int Remaining { get; set; }

        [JsonPropertyName("reset_after")]
        public int ResetAfter { get; set; }

        [JsonPropertyName("max_concurrency")]
        public int MaxConcurrency { get; set; }
    }
}
