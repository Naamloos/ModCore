using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record RoleColors
    {
        [JsonPropertyName("primary_color")]
        public int PrimaryColor { get; set; }

        [JsonPropertyName("secondary_color")]
        public int? SecondaryColor { get; set; }

        [JsonPropertyName("tertiary_color")]
        public int? TertiaryColor { get; set; }
    }
}
