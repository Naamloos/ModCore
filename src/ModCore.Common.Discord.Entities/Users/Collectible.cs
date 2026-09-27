using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Users
{
    public record Collectible
    {
        [JsonPropertyName("nameplate")]
        public Optional<Nameplate> Nameplate { get; set; }
    }
}
