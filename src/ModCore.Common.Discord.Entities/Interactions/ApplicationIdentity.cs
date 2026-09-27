using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record ApplicationIdentity
    {
        [JsonPropertyName("user_id")]
        public Optional<Snowflake> UserId { get; set; }

        [JsonPropertyName("provider_type")]
        public string ProviderType { get; set; } = default!;

        [JsonPropertyName("provider_id")]
        public Optional<string> ProviderId { get; set; }

        [JsonPropertyName("provider_issued_user_id")]
        public string ProviderIssuedUserId { get; set; } = default!;
    }
}
