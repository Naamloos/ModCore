using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Components;
using ModCore.Common.Discord.Entities.Enums;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record InteractionData
    {
        [JsonPropertyName("id")]
        public Snowflake CommandId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public ApplicationCommandType Type { get; set; }

        [JsonPropertyName("resolved")]
        public Optional<ResolvedDataStructure> Resolved { get; set; }

        [JsonPropertyName("options")]
        public Optional<List<ApplicationCommandInteractionDataOption>> Options { get; set; }

        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }

        [JsonPropertyName("target_id")]
        public Optional<Snowflake> TargetId { get; set; }

        [JsonPropertyName("custom_id")]
        public Optional<string> CustomId { get; set; }

        [JsonPropertyName("component_type")]
        public Optional<ComponentType> ComponentType { get; set; }

        [JsonPropertyName("values")]
        public Optional<string[]> Values { get; set; }

        [JsonPropertyName("components")]
        public Optional<Component[]> Components { get; set; }
    }
}
