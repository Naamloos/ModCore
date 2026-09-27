using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Gateway
{
    public record GatewayBot
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = default!;

        [JsonPropertyName("shards")]
        public int Shards { get; set; }

        [JsonPropertyName("session_start_limit")]
        public SessionStartLimit SessionStartLimit { get; set; } = default!;
    }
}
