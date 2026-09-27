using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record InstallParams
    {
        [JsonPropertyName("scopes")]
        public string[] Scopes { get; set; } = default!;

        [JsonPropertyName("permissions")]
        public string Permissions { get; set; } = default!;
    }
}
