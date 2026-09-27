using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Interactions;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record GetGlobalApplicationCommandsQuery
    {
        [JsonPropertyName("with_localizations")]
        public Optional<bool> WithLocalizations { get; set; }
    }

    public record CreateGlobalApplicationCommandRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("name_localizations")]
        public Optional<Dictionary<string, string>?> NameLocalizations { get; set; }

        [JsonPropertyName("description")]
        public Optional<string> Description { get; set; }

        [JsonPropertyName("description_localizations")]
        public Optional<Dictionary<string, string>?> DescriptionLocalizations { get; set; }

        [JsonPropertyName("options")]
        public Optional<ApplicationCommandOption[]> Options { get; set; }

        [JsonPropertyName("default_member_permissions")]
        public Optional<string?> DefaultMemberPermissions { get; set; }

        [JsonPropertyName("dm_permission")]
        public Optional<bool?> DmPermission { get; set; }

        [JsonPropertyName("default_permission")]
        public Optional<bool> DefaultPermission { get; set; }

        [JsonPropertyName("integration_types")]
        public Optional<int[]> IntegrationTypes { get; set; }

        [JsonPropertyName("contexts")]
        public Optional<int[]> Contexts { get; set; }

        [JsonPropertyName("type")]
        public Optional<int> Type { get; set; }

        [JsonPropertyName("nsfw")]
        public Optional<bool> Nsfw { get; set; }

        [JsonPropertyName("handler")]
        public Optional<int> Handler { get; set; }
    }

    public record EditGlobalApplicationCommandRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("name_localizations")]
        public Optional<Dictionary<string, string>?> NameLocalizations { get; set; }

        [JsonPropertyName("description")]
        public Optional<string> Description { get; set; }

        [JsonPropertyName("description_localizations")]
        public Optional<Dictionary<string, string>?> DescriptionLocalizations { get; set; }

        [JsonPropertyName("options")]
        public Optional<ApplicationCommandOption[]> Options { get; set; }

        [JsonPropertyName("default_member_permissions")]
        public Optional<string?> DefaultMemberPermissions { get; set; }

        [JsonPropertyName("dm_permission")]
        public Optional<bool?> DmPermission { get; set; }

        [JsonPropertyName("default_permission")]
        public Optional<bool> DefaultPermission { get; set; }

        [JsonPropertyName("integration_types")]
        public Optional<int[]> IntegrationTypes { get; set; }

        [JsonPropertyName("contexts")]
        public Optional<int[]> Contexts { get; set; }

        [JsonPropertyName("nsfw")]
        public Optional<bool> Nsfw { get; set; }

        [JsonPropertyName("handler")]
        public Optional<int> Handler { get; set; }
    }

    public record GetGuildApplicationCommandsQuery
    {
        [JsonPropertyName("with_localizations")]
        public Optional<bool> WithLocalizations { get; set; }
    }

    public record CreateGuildApplicationCommandRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("name_localizations")]
        public Optional<Dictionary<string, string>?> NameLocalizations { get; set; }

        [JsonPropertyName("description")]
        public Optional<string> Description { get; set; }

        [JsonPropertyName("description_localizations")]
        public Optional<Dictionary<string, string>?> DescriptionLocalizations { get; set; }

        [JsonPropertyName("options")]
        public Optional<ApplicationCommandOption[]> Options { get; set; }

        [JsonPropertyName("default_member_permissions")]
        public Optional<string?> DefaultMemberPermissions { get; set; }

        [JsonPropertyName("default_permission")]
        public Optional<bool> DefaultPermission { get; set; }

        [JsonPropertyName("type")]
        public Optional<int> Type { get; set; }

        [JsonPropertyName("nsfw")]
        public Optional<bool> Nsfw { get; set; }
    }

    public record EditGuildApplicationCommandRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("name_localizations")]
        public Optional<Dictionary<string, string>?> NameLocalizations { get; set; }

        [JsonPropertyName("description")]
        public Optional<string> Description { get; set; }

        [JsonPropertyName("description_localizations")]
        public Optional<Dictionary<string, string>?> DescriptionLocalizations { get; set; }

        [JsonPropertyName("options")]
        public Optional<ApplicationCommandOption[]> Options { get; set; }

        [JsonPropertyName("default_member_permissions")]
        public Optional<string?> DefaultMemberPermissions { get; set; }

        [JsonPropertyName("default_permission")]
        public Optional<bool> DefaultPermission { get; set; }

        [JsonPropertyName("nsfw")]
        public Optional<bool> Nsfw { get; set; }
    }

    public record BulkOverwriteGuildApplicationCommandsRequest
    {
        [JsonPropertyName("id")]
        public Optional<Snowflake> Id { get; set; }

        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("name_localizations")]
        public Optional<Dictionary<string, string>?> NameLocalizations { get; set; }

        [JsonPropertyName("description")]
        public Optional<string> Description { get; set; }

        [JsonPropertyName("description_localizations")]
        public Optional<Dictionary<string, string>?> DescriptionLocalizations { get; set; }

        [JsonPropertyName("options")]
        public Optional<ApplicationCommandOption[]> Options { get; set; }

        [JsonPropertyName("default_member_permissions")]
        public Optional<string?> DefaultMemberPermissions { get; set; }

        [JsonPropertyName("dm_permission")]
        public Optional<bool?> DmPermission { get; set; }

        [JsonPropertyName("default_permission")]
        public Optional<bool> DefaultPermission { get; set; }

        [JsonPropertyName("integration_types")]
        public Optional<int[]> IntegrationTypes { get; set; }

        [JsonPropertyName("contexts")]
        public Optional<int[]> Contexts { get; set; }

        [JsonPropertyName("type")]
        public Optional<int> Type { get; set; }
    }

    public record EditApplicationCommandPermissionsRequest
    {
        [JsonPropertyName("permissions")]
        public Optional<ApplicationCommandPermissions[]> Permissions { get; set; }
    }
}
