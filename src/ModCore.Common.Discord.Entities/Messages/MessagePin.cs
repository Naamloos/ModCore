using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record MessagePin
    {
        [JsonPropertyName("pinned_at")]
        public DateTimeOffset PinnedAt { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; } = default!;
    }
}
