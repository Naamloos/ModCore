using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Gateway
{
    public record RequestGuildMemberRateLimitMetadata
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("nonce")]
        public Optional<string> Nonce { get; set; }
    }
}
