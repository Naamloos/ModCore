using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record ApplicationIdentityProfile
    {
        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("metadata")]
        public JsonElement? Metadata { get; set; }

        [JsonPropertyName("data")]
        public ProfileData? Data { get; set; }
    }
}
