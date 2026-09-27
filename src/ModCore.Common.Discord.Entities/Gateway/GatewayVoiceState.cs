using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Gateway
{
    public record GatewayVoiceState
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("channel_id")]
        public Snowflake? ChannelId { get; set; }

        [JsonPropertyName("self_mute")]
        public bool SelfMute { get; set; }

        [JsonPropertyName("self_deaf")]
        public bool SelfDeaf { get; set; }
    }
}
