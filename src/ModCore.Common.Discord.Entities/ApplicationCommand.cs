using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities
{
    public class ApplicationCommand
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("type")]
        public Optional<ApplicationCommandType> Type { get; set; }

        [JsonPropertyName("application_id")]
        public Snowflake ApplicationId { get; set; }

        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("name_localizations")]
        public Optional<Dictionary<string, string>?> NameLocalizations { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("decription_localizations")]
        public Optional<Dictionary<string, string>?> DescriptionLocalizations { get; set; }

        [JsonPropertyName("options")]
        public Optional<JsonObject[]> Options { get; set; }

        [JsonPropertyName("default_member_permissions")]
        public string? DefaultMemberPermissions { get; set; }

        [JsonPropertyName("dm_permission")]
        public Optional<bool> CanBeUsedInDM { get; set; }

        [JsonPropertyName("nsfw")]
        public Optional<bool> NSFW { get; set; }

        [JsonPropertyName("version")]
        public Snowflake Version { get; set; }
    }

    public enum ApplicationCommandType : int
    {
        ChatInput = 1,
        User = 2,
        Message = 3
    }
}
