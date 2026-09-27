using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Guilds;

namespace ModCore.Common.Discord.Entities.Voice
{
    public record VoiceState
    {
        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }

        [JsonPropertyName("channel_id")]
        public Snowflake? ChannelId { get; set; }

        [JsonPropertyName("user_id")]
        public Snowflake UserId { get; set; }

        [JsonPropertyName("member")]
        public Optional<Member> Member { get; set; }

        [JsonPropertyName("session_id")]
        public string SessionId { get; set; } = default!;

        [JsonPropertyName("deaf")]
        public bool Deaf { get; set; }

        [JsonPropertyName("mute")]
        public bool Mute { get; set; }

        [JsonPropertyName("self_deaf")]
        public bool SelfDeaf { get; set; }

        [JsonPropertyName("self_mute")]
        public bool SelfMute { get; set; }

        [JsonPropertyName("self_stream")]
        public Optional<bool> SelfStream { get; set; }

        [JsonPropertyName("self_video")]
        public bool SelfVideo { get; set; }

        [JsonPropertyName("suppress")]
        public bool Suppress { get; set; }

        [JsonPropertyName("request_to_speak_timestamp")]
        public DateTimeOffset? RequestToSpeakTimestamp { get; set; }
    }
}
