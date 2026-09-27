using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record ProfileData
    {
        [JsonPropertyName("primary")]
        public Optional<PrimaryProfileData> Primary { get; set; }

        [JsonPropertyName("dynamic")]
        public Optional<JsonElement[]> Dynamic { get; set; }
    }
}
