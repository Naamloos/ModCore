using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Channels;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Messages;

namespace ModCore.Common.Discord.Entities
{
    public record ResolvedDataStructure
    {
        [JsonPropertyName("users")]
        public Optional<Dictionary<string, User>> Users { get; set; }

        [JsonPropertyName("members")]
        public Optional<Dictionary<string, Member>> Members { get; set; }

        [JsonPropertyName("channels")]
        public Optional<Dictionary<string, Channel>> Channel { get; set; }

        [JsonPropertyName("messages")]
        public Optional<Dictionary<string, Message>> Messages { get; set; }

        [JsonPropertyName("attachments")]
        public Optional<Dictionary<string, Attachment>> Attachments { get; set; }
    }
}