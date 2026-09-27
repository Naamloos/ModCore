using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Interactions;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record UpdateApplicationIdentityProfileRequest
    {
        [JsonPropertyName("username")]
        public Optional<string> Username { get; set; }

        [JsonPropertyName("data")]
        public Optional<ProfileData> Data { get; set; }
    }

    public record GetApplicationIdentitiesByExternalIDQuery
    {
        [JsonPropertyName("provider_id")]
        public Optional<string> ProviderId { get; set; }
    }

    public record DeleteApplicationIdentityRequest
    {
        [JsonPropertyName("provider_id")]
        public Optional<string> ProviderId { get; set; }
    }
}
