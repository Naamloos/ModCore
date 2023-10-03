using ModCore.Common.Discord.Entities.Enums;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record ChannelMention
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("type")]
        public ChannelType Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}