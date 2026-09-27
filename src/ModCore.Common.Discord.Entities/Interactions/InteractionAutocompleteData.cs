using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record InteractionAutocompleteData : InteractionResponseData
    {
        [JsonPropertyName("choices")]
        public ApplicationCommandOptionChoice[] Choices { get; set; } = default!;
    }
}
