using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Guilds;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record PollMedia
    {
        [JsonPropertyName("text")]
        public Optional<string> Text { get; set; }

        [JsonPropertyName("emoji")]
        public Optional<Emoji> Emoji { get; set; }
    }
}
