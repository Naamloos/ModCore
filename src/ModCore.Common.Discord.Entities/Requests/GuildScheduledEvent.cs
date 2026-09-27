using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Guilds;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record ListScheduledEventsForGuildQuery
    {
        [JsonPropertyName("with_user_count")]
        public Optional<bool> WithUserCount { get; set; }
    }

    public record CreateGuildScheduledEventRequest
    {
        [JsonPropertyName("channel_id")]
        public Optional<Snowflake> ChannelId { get; set; }

        [JsonPropertyName("entity_metadata")]
        public Optional<GuildScheduledEventEntityMetadata> EntityMetadata { get; set; }

        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("privacy_level")]
        public Optional<int> PrivacyLevel { get; set; }

        [JsonPropertyName("scheduled_start_time")]
        public Optional<DateTimeOffset> ScheduledStartTime { get; set; }

        [JsonPropertyName("scheduled_end_time")]
        public Optional<DateTimeOffset> ScheduledEndTime { get; set; }

        [JsonPropertyName("description")]
        public Optional<string> Description { get; set; }

        [JsonPropertyName("entity_type")]
        public Optional<int> EntityType { get; set; }

        [JsonPropertyName("image")]
        public Optional<string> Image { get; set; }

        [JsonPropertyName("recurrence_rule")]
        public Optional<GuildScheduledEventRecurrenceRule> RecurrenceRule { get; set; }
    }

    public record GetGuildScheduledEventQuery
    {
        [JsonPropertyName("with_user_count")]
        public Optional<bool> WithUserCount { get; set; }
    }

    public record ModifyGuildScheduledEventRequest
    {
        [JsonPropertyName("channel_id")]
        public Optional<Snowflake?> ChannelId { get; set; }

        [JsonPropertyName("entity_metadata")]
        public Optional<GuildScheduledEventEntityMetadata?> EntityMetadata { get; set; }

        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("privacy_level")]
        public Optional<int> PrivacyLevel { get; set; }

        [JsonPropertyName("scheduled_start_time")]
        public Optional<DateTimeOffset> ScheduledStartTime { get; set; }

        [JsonPropertyName("scheduled_end_time")]
        public Optional<DateTimeOffset> ScheduledEndTime { get; set; }

        [JsonPropertyName("description")]
        public Optional<string?> Description { get; set; }

        [JsonPropertyName("entity_type")]
        public Optional<int> EntityType { get; set; }

        [JsonPropertyName("status")]
        public Optional<int> Status { get; set; }

        [JsonPropertyName("image")]
        public Optional<string> Image { get; set; }

        [JsonPropertyName("recurrence_rule")]
        public Optional<GuildScheduledEventRecurrenceRule?> RecurrenceRule { get; set; }
    }

    public record GetGuildScheduledEventUsersQuery
    {
        [JsonPropertyName("limit")]
        public Optional<double> Limit { get; set; }

        [JsonPropertyName("with_member")]
        public Optional<bool> WithMember { get; set; }

        [JsonPropertyName("before")]
        public Optional<Snowflake> Before { get; set; }

        [JsonPropertyName("after")]
        public Optional<Snowflake> After { get; set; }
    }
}
