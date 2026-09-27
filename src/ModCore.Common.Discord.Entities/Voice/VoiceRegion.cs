using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Voice
{
    public record VoiceRegion
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("optimal")]
        public bool Optimal { get; set; }

        [JsonPropertyName("deprecated")]
        public bool Deprecated { get; set; }

        [JsonPropertyName("custom")]
        public bool Custom { get; set; }
    }
}
