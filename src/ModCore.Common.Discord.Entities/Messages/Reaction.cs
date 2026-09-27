using System.Text.Json;
using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Guilds;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record Reaction
    {
        [JsonPropertyName("emoji_id")]
        public Snowflake? EmojiId { get; set; }

        [JsonPropertyName("emoji_name")]
        public string? EmojiName { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("count_details")]
        public ReactionCountDetails CountDetails { get; set; } = default!;

        [JsonPropertyName("me")]
        public bool Me { get; set; }

        [JsonPropertyName("me_burst")]
        public bool MeBurst { get; set; }

        [JsonPropertyName("emoji")]
        public Emoji Emoji { get; set; } = default!;

        [JsonPropertyName("burst_colors")]
        public JsonElement[] BurstColors { get; set; } = default!;
    }
}
