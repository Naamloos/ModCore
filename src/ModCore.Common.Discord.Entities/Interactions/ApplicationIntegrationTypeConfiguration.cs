using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record ApplicationIntegrationTypeConfiguration
    {
        [JsonPropertyName("oauth2_install_params")]
        public Optional<InstallParams> Oauth2InstallParams { get; set; }
    }
}
