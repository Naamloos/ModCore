using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record ActivityLocation
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("kind")]
        public int Kind { get; set; }

        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; set; }

        [JsonPropertyName("guild_id")]
        public Optional<Snowflake?> GuildId { get; set; }
    }
}
