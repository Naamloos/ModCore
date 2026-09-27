using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record PollResults
    {
        [JsonPropertyName("is_finalized")]
        public bool IsFinalized { get; set; }

        [JsonPropertyName("answer_counts")]
        public PollAnswerCount[] AnswerCounts { get; set; } = default!;
    }
}
