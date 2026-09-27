using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record RequestGuildMembers
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("query")]
        public Optional<string> Query { get; set; }

        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("presences")]
        public Optional<bool> Presences { get; set; }

        [JsonPropertyName("user_ids")]
        public Optional<Snowflake[]> UserIds { get; set; }

        [JsonPropertyName("nonce")]
        public Optional<string> Nonce { get; set; }
    }
}
