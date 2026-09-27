using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Guilds;

namespace ModCore.Common.Discord.Entities.Interactions
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

        [JsonPropertyName("rpc_origins")]
        public Optional<string[]> RpcOrigins { get; set; }

        [JsonPropertyName("bot")]
        public Optional<User> Bot { get; set; }

        [JsonPropertyName("verify_key")]
        public string VerifyKey { get; set; } = default!;

        [JsonPropertyName("primary_sku_id")]
        public Optional<Snowflake> PrimarySkuId { get; set; }

        [JsonPropertyName("slug")]
        public Optional<string> Slug { get; set; }

        [JsonPropertyName("flags_new")]
        public Optional<string> FlagsNew { get; set; }

        [JsonPropertyName("approximate_guild_count")]
        public Optional<int> ApproximateGuildCount { get; set; }

        [JsonPropertyName("approximate_user_install_count")]
        public Optional<int> ApproximateUserInstallCount { get; set; }

        [JsonPropertyName("approximate_user_authorization_count")]
        public Optional<int> ApproximateUserAuthorizationCount { get; set; }

        [JsonPropertyName("redirect_uris")]
        public Optional<string[]> RedirectUris { get; set; }

        [JsonPropertyName("interactions_endpoint_url")]
        public Optional<string?> InteractionsEndpointUrl { get; set; }

        [JsonPropertyName("role_connections_verification_url")]
        public Optional<string?> RoleConnectionsVerificationUrl { get; set; }

        [JsonPropertyName("event_webhooks_url")]
        public Optional<string?> EventWebhooksUrl { get; set; }

        [JsonPropertyName("event_webhooks_status")]
        public Optional<int> EventWebhooksStatus { get; set; }

        [JsonPropertyName("event_webhooks_types")]
        public Optional<string[]> EventWebhooksTypes { get; set; }

        [JsonPropertyName("install_params")]
        public Optional<InstallParams> InstallParams { get; set; }

        [JsonPropertyName("integration_types_config")]
        public Optional<
            Dictionary<string, ApplicationIntegrationTypeConfiguration>
        > IntegrationTypesConfig { get; set; }

        [JsonPropertyName("custom_install_url")]
        public Optional<string> CustomInstallUrl { get; set; }
    }
}
