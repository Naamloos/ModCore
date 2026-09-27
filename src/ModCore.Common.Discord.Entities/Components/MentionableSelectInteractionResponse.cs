using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Enums;

namespace ModCore.Common.Discord.Entities.Components
{
    public class MentionableSelectInteractionResponse
    {
        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("component_type")]
        public int ComponentType { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("custom_id")]
        public string CustomId { get; set; } = default!;

        [JsonPropertyName("resolved")]
        public ResolvedDataStructure Resolved { get; set; } = default!;

        [JsonPropertyName("values")]
        public Snowflake[] Values { get; set; } = default!;
    }
}
