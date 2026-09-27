using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Voice
{
    public record SoundboardSound
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("sound_id")]
        public Snowflake SoundId { get; set; }

        [JsonPropertyName("volume")]
        public double Volume { get; set; }

        [JsonPropertyName("emoji_id")]
        public Snowflake? EmojiId { get; set; }

        [JsonPropertyName("emoji_name")]
        public string? EmojiName { get; set; }

        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }

        [JsonPropertyName("available")]
        public bool Available { get; set; }

        [JsonPropertyName("user")]
        public Optional<User> User { get; set; }
    }
}
