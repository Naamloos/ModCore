using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Gateway
{
    public record GatewayPresenceEvent
    {
        [JsonPropertyName("user")]
        public User User { get; set; } = default!;

        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = default!;

        [JsonPropertyName("activities")]
        public GatewayActivity[] Activities { get; set; } = default!;

        [JsonPropertyName("client_status")]
        public ClientStatus ClientStatus { get; set; } = default!;
    }
}
