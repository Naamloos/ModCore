using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Responses
{
    public record GetAnswerVotersResponse
    {
        [JsonPropertyName("users")]
        public User[] Users { get; set; } = default!;
    }
}
