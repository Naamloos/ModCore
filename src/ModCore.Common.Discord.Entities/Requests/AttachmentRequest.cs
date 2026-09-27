using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record AttachmentRequest
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("filename")]
        public Optional<string> Filename { get; set; }

        [JsonPropertyName("title")]
        public Optional<string> Title { get; set; }

        [JsonPropertyName("description")]
        public Optional<string> Description { get; set; }

        [JsonPropertyName("duration_secs")]
        public Optional<double> DurationSecs { get; set; }

        [JsonPropertyName("waveform")]
        public Optional<string> Waveform { get; set; }

        [JsonPropertyName("is_spoiler")]
        public Optional<bool> IsSpoiler { get; set; }
    }
}
