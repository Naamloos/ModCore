using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record GetInviteQuery
    {
        [JsonPropertyName("with_counts")]
        public Optional<bool> WithCounts { get; set; }

        [JsonPropertyName("guild_scheduled_event_id")]
        public Optional<Snowflake> GuildScheduledEventId { get; set; }
    }

    public record UpdateTargetUsersRequest { }
}
