using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record MessageCall
    {
        [JsonPropertyName("participants")]
        public Snowflake[] Participants { get; set; } = default!;

        [JsonPropertyName("ended_timestamp")]
        public Optional<DateTimeOffset?> EndedTimestamp { get; set; }
    }
}
