using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Lobbies
{
    public record LobbyInvite
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = default!;
    }
}
