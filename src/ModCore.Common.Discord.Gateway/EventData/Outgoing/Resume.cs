using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Gateway.EventData.Outgoing
{
    internal record Resume
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }

        [JsonPropertyName("seq")]
        public int LastSequenceNumber { get; set; }
    }
}
