using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Gateway
{
    public record GatewayPresence
    {
        [JsonPropertyName("since")]
        public long? Since { get; set; }

        [JsonPropertyName("activities")]
        public GatewayActivity[] Activities { get; set; } = default!;

        [JsonPropertyName("status")]
        public string Status { get; set; } = default!;

        [JsonPropertyName("afk")]
        public bool Afk { get; set; }
    }
}
