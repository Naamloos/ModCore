using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities
{
    public record ThreadMetadata
    {
        [JsonPropertyName("archived")]
        public bool Archived { get; set; }

        [JsonPropertyName("auto_archive_duration")]
        public int AutoArchiveDuration { get; set; }

        [JsonPropertyName("archive_timestamp")]
        public DateTimeOffset ArchiveTimestamp { get; set; }

        [JsonPropertyName("locked")]
        public bool Locked { get; set; }

        [JsonPropertyName("invitable")]
        public bool Invitable { get; set; }

        [JsonPropertyName("create_timestamp")]
        public Optional<DateTimeOffset> CreateTimestamp { get; set; }
    }
}