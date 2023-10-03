using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record MessageReference
    {
        [JsonPropertyName("message_id")]
        public Optional<Snowflake> MessageId { get; set; }

        [JsonPropertyName("channel_id")]
        public Optional<Snowflake> ChannelId { get; set; }

        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }

        [JsonPropertyName("fail_if_not_exists")]
        public Optional<bool> FailIfNotExists { get; set; }
    }
}