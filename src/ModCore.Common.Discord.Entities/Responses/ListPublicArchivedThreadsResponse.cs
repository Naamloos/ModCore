using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Channels;

namespace ModCore.Common.Discord.Entities.Responses
{
    public record ListPublicArchivedThreadsResponse
    {
        [JsonPropertyName("threads")]
        public Channel[] Threads { get; set; } = default!;

        [JsonPropertyName("members")]
        public ThreadMember[] Members { get; set; } = default!;

        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }
    }
}
