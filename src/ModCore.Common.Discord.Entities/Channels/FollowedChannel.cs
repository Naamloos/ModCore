using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Channels
{
    public record FollowedChannel
    {
        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; set; }

        [JsonPropertyName("webhook_id")]
        public Snowflake WebhookId { get; set; }
    }
}
