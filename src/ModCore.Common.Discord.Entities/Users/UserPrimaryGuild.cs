using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Users
{
    public record UserPrimaryGuild
    {
        [JsonPropertyName("identity_guild_id")]
        public Snowflake? IdentityGuildId { get; set; }

        [JsonPropertyName("identity_enabled")]
        public bool? IdentityEnabled { get; set; }

        [JsonPropertyName("tag")]
        public string? Tag { get; set; }

        [JsonPropertyName("badge")]
        public string? Badge { get; set; }
    }
}
