using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record RequestSoundboardSounds
    {
        [JsonPropertyName("guild_ids")]
        public Snowflake[] GuildIds { get; set; } = default!;
    }
}
