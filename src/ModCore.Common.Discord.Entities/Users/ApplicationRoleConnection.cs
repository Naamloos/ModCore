using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Users
{
    public record ApplicationRoleConnection
    {
        [JsonPropertyName("platform_name")]
        public string? PlatformName { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata { get; set; } = default!;
    }
}
