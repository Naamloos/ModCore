using System.Text.Json.Serialization;

namespace ModCore.Services.Shard
{
    /// <summary>
    /// Services will just use the IConfiguration utility to fetch info from this, 
    /// but to create a template file we want this class to define what properties we provide.
    /// </summary>
    public class Settings
    {
        [JsonPropertyName("discord_token")]
        public string Token { get; set; } = "";

        [JsonPropertyName("shard_count")]
        public int ShardCount { get; set; } = 1;

        [JsonPropertyName("current_shard")]
        public int CurrentShard { get; set; } = 0;
    }
}
