using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record PollAnswerCount
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("me_voted")]
        public bool MeVoted { get; set; }
    }
}
