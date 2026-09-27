using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record Attachment
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("filename")]
        public string Filename { get; set; }

        [JsonPropertyName("description")]
        public Optional<string> Description { get; set; }

        [JsonPropertyName("content_type")]
        public Optional<string> ContentType { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("proxy_url")]
        public string ProxyUrl { get; set; }

        [JsonPropertyName("height")]
        public Optional<int?> Height { get; set; }

        [JsonPropertyName("width")]
        public Optional<int?> Width { get; set; }

        [JsonPropertyName("ephemeral")]
        public Optional<bool> Ephemeral { get; set; }

        [JsonPropertyName("duration_seconds")]
        public Optional<float> DurationSeconds { get; set; }

        [JsonPropertyName("waveform")]
        public Optional<string> WaveForm { get; set; }

        [JsonPropertyName("flags")]
        public Optional<AttachmentFlags> Flags { get; set; }

        [JsonPropertyName("title")]
        public Optional<string> Title { get; set; }

        [JsonPropertyName("placeholder")]
        public Optional<string> Placeholder { get; set; }

        [JsonPropertyName("placeholder_version")]
        public Optional<int> PlaceholderVersion { get; set; }

        [JsonPropertyName("duration_secs")]
        public Optional<double> DurationSecs { get; set; }

        [JsonPropertyName("clip_participants")]
        public Optional<User[]> ClipParticipants { get; set; }

        [JsonPropertyName("clip_created_at")]
        public Optional<DateTimeOffset> ClipCreatedAt { get; set; }

        [JsonPropertyName("application")]
        public Optional<Application?> Application { get; set; }
    }
}
