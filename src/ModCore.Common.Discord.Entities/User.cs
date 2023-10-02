using ModCore.Common.Discord.Entities;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Rest.Entities
{
    public class User
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }
    }
}
