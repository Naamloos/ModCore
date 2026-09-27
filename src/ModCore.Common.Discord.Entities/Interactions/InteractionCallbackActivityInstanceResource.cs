using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record InteractionCallbackActivityInstanceResource
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;
    }
}
