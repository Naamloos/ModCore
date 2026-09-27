using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record RequestChannelInfo
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("fields")]
        public string[] Fields { get; set; } = default!;
    }
}
