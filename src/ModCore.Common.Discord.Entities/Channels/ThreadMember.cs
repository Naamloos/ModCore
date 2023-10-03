using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Guilds;

namespace ModCore.Common.Discord.Entities.Channels
{
    public record ThreadMember
    {
        [JsonPropertyName("id")]
        public Optional<Snowflake> Id { get; set; }

        [JsonPropertyName("user_id")]
        public Optional<Snowflake> UserId { get; set; }

        [JsonPropertyName("join_timestamp")]
        public DateTimeOffset JoinTimestamp { get; set; }

        [JsonPropertyName("flags")]
        public int Flags { get; set; }

        [JsonPropertyName("member")]
        public Optional<Member> Member { get; set; }
    }
}