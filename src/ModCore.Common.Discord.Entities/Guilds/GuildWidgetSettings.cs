using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record GuildWidgetSettings
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("channel_id")]
        public Snowflake? ChannelId { get; set; }
    }
}
