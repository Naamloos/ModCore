using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record ReactionCountDetails
    {
        [JsonPropertyName("burst")]
        public int Burst { get; set; }

        [JsonPropertyName("normal")]
        public int Normal { get; set; }
    }
}
