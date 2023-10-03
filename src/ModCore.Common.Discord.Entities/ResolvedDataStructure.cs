using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Channels;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Messages;

namespace ModCore.Common.Discord.Entities
{
    public record ResolvedDataStructure
    {
        [JsonPropertyName("users")]
        public Optional<Dictionary<Snowflake, User>> Users { get; set; }

        [JsonPropertyName("members")]
        public Optional<Dictionary<Snowflake, Member>> Members { get; set; }

        [JsonPropertyName("channels")]
        public Optional<Dictionary<Snowflake, Channel>> Channel { get; set; }

        [JsonPropertyName("messages")]
        public Optional<Dictionary<Snowflake, Message>> Messages { get; set; }

        [JsonPropertyName("attachments")]
        public Optional<Dictionary<Snowflake, Attachment>> Attachments { get; set; }
    }
}