using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Enums;

namespace ModCore.Common.Discord.Entities.Components
{
    public class FileUpload : Component
    {
        [JsonPropertyName("type")]
        public override ComponentType Type { get; set; } = (ComponentType)19;

        [JsonPropertyName("custom_id")]
        public string CustomId { get; set; } = default!;

        [JsonPropertyName("min_values")]
        public Optional<int> MinValues { get; set; }

        [JsonPropertyName("max_values")]
        public Optional<int> MaxValues { get; set; }

        [JsonPropertyName("required")]
        public Optional<bool> Required { get; set; }

        [JsonPropertyName("file_types")]
        public Optional<string[]> FileTypes { get; set; }
    }
}
