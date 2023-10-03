﻿using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities
{
    public record DefaultReaction
    {
        [JsonPropertyName("emoji_id")]
        public Snowflake? EmojiId { get; set; }

        [JsonPropertyName("emoji_name")]
        public string? EmojiName { get; set; }
    }
}