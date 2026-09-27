using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record SendSoundboardSoundRequest
    {
        [JsonPropertyName("sound_id")]
        public Optional<Snowflake> SoundId { get; set; }

        [JsonPropertyName("source_guild_id")]
        public Optional<Snowflake> SourceGuildId { get; set; }
    }

    public record CreateGuildSoundboardSoundRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("sound")]
        public Optional<string> Sound { get; set; }

        [JsonPropertyName("volume")]
        public Optional<double?> Volume { get; set; }

        [JsonPropertyName("emoji_id")]
        public Optional<Snowflake?> EmojiId { get; set; }

        [JsonPropertyName("emoji_name")]
        public Optional<string?> EmojiName { get; set; }
    }

    public record ModifyGuildSoundboardSoundRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("volume")]
        public Optional<double?> Volume { get; set; }

        [JsonPropertyName("emoji_id")]
        public Optional<Snowflake?> EmojiId { get; set; }

        [JsonPropertyName("emoji_name")]
        public Optional<string?> EmojiName { get; set; }
    }
}
