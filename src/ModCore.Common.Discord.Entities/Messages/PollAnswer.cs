using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record PollAnswer
    {
        [JsonPropertyName("answer_id")]
        public int AnswerId { get; set; }

        [JsonPropertyName("poll_media")]
        public PollMedia PollMedia { get; set; } = default!;
    }
}
