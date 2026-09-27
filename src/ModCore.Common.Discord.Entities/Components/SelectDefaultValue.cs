using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Components
{
    public class SelectDefaultValue
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = default!;
    }
}
