using ModCore.Common.Discord.Entities.Enums;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record ApplicationCommandOption
    {
        [JsonPropertyName("type")]
        public ApplicationCommandOptionType Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("name_localizations")]
        public Optional<Dictionary<string, string>?> NameLocalizations { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("decription_localizations")]
        public Optional<Dictionary<string, string>?> DescriptionLocalizations { get; set; }

        [JsonPropertyName("required")]
        public Optional<bool> Required { get; set; }

        [JsonPropertyName("choices")]
        public Optional<List<ApplicationCommandOptionChoice>> Choices { get; set; }

        [JsonPropertyName("options")]
        public Optional<List<ApplicationCommandOption>> Options { get; set; }

        [JsonPropertyName("channel_types")]
        public Optional<List<ChannelType>> ChannelTypes { get; set; }

        [JsonPropertyName("min_value")]
        public Optional<double> MinValue { get; set; }

        [JsonPropertyName("max_value")]
        public Optional<double> MaxValue { get; set; }

        [JsonPropertyName("min_length")]
        public Optional<int> MinLength { get; set; }

        [JsonPropertyName("max_length")]
        public Optional<int> MaxLength { get; set; }

        [JsonPropertyName("autocomplete")]
        public Optional<bool> Autocomplete { get; set; }
    }
}