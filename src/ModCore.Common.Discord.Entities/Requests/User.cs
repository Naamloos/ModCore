using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record ModifyCurrentUserRequest
    {
        [JsonPropertyName("username")]
        public Optional<string> Username { get; set; }

        [JsonPropertyName("avatar")]
        public Optional<string?> Avatar { get; set; }

        [JsonPropertyName("banner")]
        public Optional<string?> Banner { get; set; }
    }

    public record GetCurrentUserGuildsQuery
    {
        [JsonPropertyName("before")]
        public Optional<Snowflake> Before { get; set; }

        [JsonPropertyName("after")]
        public Optional<Snowflake> After { get; set; }

        [JsonPropertyName("limit")]
        public Optional<int> Limit { get; set; }

        [JsonPropertyName("with_counts")]
        public Optional<bool> WithCounts { get; set; }
    }

    public record CreateDMRequest
    {
        [JsonPropertyName("recipient_id")]
        public Optional<Snowflake> RecipientId { get; set; }
    }

    public record CreateGroupDMRequest
    {
        [JsonPropertyName("access_tokens")]
        public Optional<string[]> AccessTokens { get; set; }

        [JsonPropertyName("nicks")]
        public Optional<Dictionary<string, string>> Nicks { get; set; }
    }

    public record UpdateCurrentUserApplicationRoleConnectionRequest
    {
        [JsonPropertyName("platform_name")]
        public Optional<string> PlatformName { get; set; }

        [JsonPropertyName("platform_username")]
        public Optional<string> PlatformUsername { get; set; }

        [JsonPropertyName("metadata")]
        public Optional<Dictionary<string, string>> Metadata { get; set; }
    }
}
