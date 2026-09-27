using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Channels;
using ModCore.Common.Discord.Entities.Messages;

namespace ModCore.Common.Discord.Entities.Responses
{
    public record SearchGuildMessagesResponse
    {
        [JsonPropertyName("doing_deep_historical_index")]
        public bool DoingDeepHistoricalIndex { get; set; }

        [JsonPropertyName("documents_indexed")]
        public Optional<int> DocumentsIndexed { get; set; }

        [JsonPropertyName("total_results")]
        public int TotalResults { get; set; }

        [JsonPropertyName("messages")]
        public Message[] Messages { get; set; } = default!;

        [JsonPropertyName("threads")]
        public Optional<Channel[]> Threads { get; set; }

        [JsonPropertyName("members")]
        public Optional<ThreadMember[]> Members { get; set; }
    }
}
