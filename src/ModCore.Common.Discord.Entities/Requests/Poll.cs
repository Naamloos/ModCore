using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record GetAnswerVotersQuery
    {
        [JsonPropertyName("after")]
        public Optional<Snowflake> After { get; set; }

        [JsonPropertyName("limit")]
        public Optional<int> Limit { get; set; }
    }
}
