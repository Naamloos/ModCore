using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record GuildApplicationCommandPermissions
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("application_id")]
        public Snowflake ApplicationId { get; set; }

        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("permissions")]
        public ApplicationCommandPermissions[] Permissions { get; set; } = default!;
    }
}
