using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Channels;

namespace ModCore.Common.Discord.Entities.Responses
{
    public record ListActiveGuildThreadsResponse
    {
        [JsonPropertyName("threads")]
        public Channel[] Threads { get; set; } = default!;

        [JsonPropertyName("members")]
        public ThreadMember[] Members { get; set; } = default!;
    }
}
