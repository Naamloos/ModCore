using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record ModifyCurrentUserVoiceStateRequest
    {
        [JsonPropertyName("channel_id")]
        public Optional<Snowflake> ChannelId { get; set; }

        [JsonPropertyName("suppress")]
        public Optional<bool> Suppress { get; set; }

        [JsonPropertyName("request_to_speak_timestamp")]
        public Optional<DateTimeOffset?> RequestToSpeakTimestamp { get; set; }
    }

    public record ModifyUserVoiceStateRequest
    {
        [JsonPropertyName("channel_id")]
        public Optional<Snowflake> ChannelId { get; set; }

        [JsonPropertyName("suppress")]
        public Optional<bool> Suppress { get; set; }
    }
}
