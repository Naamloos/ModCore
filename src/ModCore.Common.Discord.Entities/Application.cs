using ModCore.Common.Discord.Rest.Entities;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities
{
    public record Application
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("icon")]
        public string? IconHash { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("bot_public")]
        public bool BotPublic { get; set; }

        [JsonPropertyName("bot_require_code_grant")]
        public bool BotRequiresCodeGrant { get; set; }

        [JsonPropertyName("terms_of_service_url")]
        public Optional<string> TermsOfServiceUrl { get; set; }

        [JsonPropertyName("privacy_policy_url")]
        public Optional<string> PrivacyPolicyUrl { get; set; }

        [JsonPropertyName("owner")]
        public Optional<User> Owner { get; set; }

        [JsonPropertyName("team")]
        public Optional<Team?> Team { get; set; }

        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }

        [JsonPropertyName("guild")]
        public Optional<Guild> Guild { get; set; }

        [JsonPropertyName("cover_image")]
        public Optional<string> CoverImage { get; set; }

        [JsonPropertyName("flags")]
        public Optional<int> Flags { get; set; }

        [JsonPropertyName("tags")]
        public Optional<string[]> Tags { get; set; }
    }
}
