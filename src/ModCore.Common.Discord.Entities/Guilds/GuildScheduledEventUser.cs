using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record GuildScheduledEventUser
    {
        [JsonPropertyName("guild_scheduled_event_id")]
        public Snowflake GuildScheduledEventId { get; set; }

        [JsonPropertyName("user")]
        public User User { get; set; } = default!;

        [JsonPropertyName("member")]
        public Optional<Member> Member { get; set; }
    }
}
