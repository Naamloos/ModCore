using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record MessageSnapshot
    {
        [JsonPropertyName("message")]
        public Message Message { get; set; } = default!;
    }
}
