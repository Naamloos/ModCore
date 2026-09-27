using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record ApplicationRoleConnectionMetadata
    {
        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; } = default!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("name_localizations")]
        public Optional<Dictionary<string, string>> NameLocalizations { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = default!;

        [JsonPropertyName("description_localizations")]
        public Optional<Dictionary<string, string>> DescriptionLocalizations { get; set; }
    }
}
