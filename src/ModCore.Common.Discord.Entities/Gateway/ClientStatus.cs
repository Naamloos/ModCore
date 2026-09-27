using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Gateway
{
    public record ClientStatus
    {
        [JsonPropertyName("desktop")]
        public Optional<string> Desktop { get; set; }

        [JsonPropertyName("mobile")]
        public Optional<string> Mobile { get; set; }

        [JsonPropertyName("web")]
        public Optional<string> Web { get; set; }

        [JsonPropertyName("vr")]
        public Optional<string> Vr { get; set; }
    }
}
