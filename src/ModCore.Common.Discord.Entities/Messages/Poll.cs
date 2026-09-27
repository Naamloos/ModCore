using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record Poll
    {
        [JsonPropertyName("question")]
        public PollMedia Question { get; set; } = default!;

        [JsonPropertyName("answers")]
        public PollAnswer[] Answers { get; set; } = default!;

        [JsonPropertyName("expiry")]
        public DateTimeOffset? Expiry { get; set; }

        [JsonPropertyName("allow_multiselect")]
        public bool AllowMultiselect { get; set; }

        [JsonPropertyName("layout_type")]
        public int LayoutType { get; set; }

        [JsonPropertyName("results")]
        public Optional<PollResults> Results { get; set; }
    }
}
