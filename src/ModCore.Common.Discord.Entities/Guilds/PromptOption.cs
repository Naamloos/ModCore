using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record PromptOption
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("channel_ids")]
        public Snowflake[] ChannelIds { get; set; } = default!;

        [JsonPropertyName("role_ids")]
        public Snowflake[] RoleIds { get; set; } = default!;

        [JsonPropertyName("emoji")]
        public Optional<Emoji> Emoji { get; set; }

        [JsonPropertyName("emoji_id")]
        public Optional<Snowflake> EmojiId { get; set; }

        [JsonPropertyName("emoji_name")]
        public Optional<string> EmojiName { get; set; }

        [JsonPropertyName("emoji_animated")]
        public Optional<bool> EmojiAnimated { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = default!;

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
