using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record ActivityInstance
    {
        [JsonPropertyName("application_id")]
        public Snowflake ApplicationId { get; set; }

        [JsonPropertyName("instance_id")]
        public string InstanceId { get; set; } = default!;

        [JsonPropertyName("launch_id")]
        public Snowflake LaunchId { get; set; }

        [JsonPropertyName("location")]
        public ActivityLocation Location { get; set; } = default!;

        [JsonPropertyName("users")]
        public Snowflake[] Users { get; set; } = default!;
    }
}
