using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Messages;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record PollCreateRequest
    {
        [JsonPropertyName("question")]
        public PollMedia Question { get; set; } = default!;

        [JsonPropertyName("answers")]
        public PollAnswer[] Answers { get; set; } = default!;

        [JsonPropertyName("duration")]
        public Optional<int> Duration { get; set; }

        [JsonPropertyName("allow_multiselect")]
        public Optional<bool> AllowMultiselect { get; set; }

        [JsonPropertyName("layout_type")]
        public Optional<int> LayoutType { get; set; }
    }
}
