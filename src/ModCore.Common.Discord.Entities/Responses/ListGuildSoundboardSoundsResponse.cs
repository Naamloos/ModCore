using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Voice;

namespace ModCore.Common.Discord.Entities.Responses
{
    public record ListGuildSoundboardSoundsResponse
    {
        [JsonPropertyName("items")]
        public SoundboardSound[] Items { get; set; } = default!;
    }
}
