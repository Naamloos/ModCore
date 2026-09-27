using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Interactions;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record EditCurrentApplicationRequest
    {
        [JsonPropertyName("custom_install_url")]
        public Optional<string> CustomInstallUrl { get; set; }

        [JsonPropertyName("description")]
        public Optional<string> Description { get; set; }

        [JsonPropertyName("role_connections_verification_url")]
        public Optional<string> RoleConnectionsVerificationUrl { get; set; }

        [JsonPropertyName("install_params")]
        public Optional<InstallParams> InstallParams { get; set; }

        [JsonPropertyName("integration_types_config")]
        public Optional<
            Dictionary<string, ApplicationIntegrationTypeConfiguration>
        > IntegrationTypesConfig { get; set; }

        [JsonPropertyName("flags")]
        public Optional<long> Flags { get; set; }

        [JsonPropertyName("icon")]
        public Optional<string?> Icon { get; set; }

        [JsonPropertyName("cover_image")]
        public Optional<string?> CoverImage { get; set; }

        [JsonPropertyName("interactions_endpoint_url")]
        public Optional<string> InteractionsEndpointUrl { get; set; }

        [JsonPropertyName("tags")]
        public Optional<string[]> Tags { get; set; }

        [JsonPropertyName("event_webhooks_url")]
        public Optional<string> EventWebhooksUrl { get; set; }

        [JsonPropertyName("event_webhooks_status")]
        public Optional<int> EventWebhooksStatus { get; set; }

        [JsonPropertyName("event_webhooks_types")]
        public Optional<string[]> EventWebhooksTypes { get; set; }
    }
}
