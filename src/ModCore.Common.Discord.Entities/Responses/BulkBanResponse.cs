using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Responses
{
    public record BulkBanResponse
    {
        [JsonPropertyName("banned_users")]
        public Snowflake[] BannedUsers { get; set; } = default!;

        [JsonPropertyName("failed_users")]
        public Snowflake[] FailedUsers { get; set; } = default!;
    }
}
