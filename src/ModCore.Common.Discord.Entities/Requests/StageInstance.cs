using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record CreateStageInstanceRequest
    {
        [JsonPropertyName("channel_id")]
        public Optional<Snowflake> ChannelId { get; set; }

        [JsonPropertyName("topic")]
        public Optional<string> Topic { get; set; }

        [JsonPropertyName("privacy_level")]
        public Optional<int> PrivacyLevel { get; set; }

        [JsonPropertyName("send_start_notification")]
        public Optional<bool> SendStartNotification { get; set; }

        [JsonPropertyName("guild_scheduled_event_id")]
        public Optional<Snowflake> GuildScheduledEventId { get; set; }
    }

    public record ModifyStageInstanceRequest
    {
        [JsonPropertyName("topic")]
        public Optional<string> Topic { get; set; }

        [JsonPropertyName("privacy_level")]
        public Optional<int> PrivacyLevel { get; set; }
    }
}
