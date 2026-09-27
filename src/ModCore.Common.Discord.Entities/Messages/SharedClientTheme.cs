using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record SharedClientTheme
    {
        [JsonPropertyName("colors")]
        public string[] Colors { get; set; } = default!;

        [JsonPropertyName("gradient_angle")]
        public int GradientAngle { get; set; }

        [JsonPropertyName("base_mix")]
        public int BaseMix { get; set; }

        [JsonPropertyName("base_theme")]
        public Optional<int?> BaseTheme { get; set; }
    }
}
