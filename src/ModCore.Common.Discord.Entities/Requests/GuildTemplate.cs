using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record CreateGuildTemplateRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("description")]
        public Optional<string?> Description { get; set; }
    }

    public record ModifyGuildTemplateRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("description")]
        public Optional<string?> Description { get; set; }
    }
}
