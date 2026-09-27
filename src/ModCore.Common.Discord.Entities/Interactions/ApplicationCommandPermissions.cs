using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record ApplicationCommandPermissions
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("permission")]
        public bool Permission { get; set; }
    }
}
