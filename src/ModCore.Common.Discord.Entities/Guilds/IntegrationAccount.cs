using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record IntegrationAccount
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;
    }
}
