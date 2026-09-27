using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record Ban
    {
        [JsonPropertyName("reason")]
        public string? Reason { get; set; }

        [JsonPropertyName("user")]
        public User User { get; set; } = default!;
    }
}
