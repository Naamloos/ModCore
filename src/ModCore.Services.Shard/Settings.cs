using System.Text.Json.Serialization;

namespace ModCore.Services.Shard
{
    /// <summary>
    /// Services will just use the IConfiguration utility to fetch info from this, 
    /// but to create a template file we want this class to define what properties we provide.
    /// </summary>
    public record Settings
    {
        [JsonPropertyName("discord_token")]
        public string Token { get; set; } = "";

        [JsonPropertyName("shard_count")]
        public int ShardCount { get; set; } = 1;

        [JsonPropertyName("current_shard")]
        public int CurrentShard { get; set; } = 0;

        [JsonPropertyName("postgres_username")]
        public string PostgresUsername { get; set; } = "postgres";

        [JsonPropertyName("postgres_password")]
        public string PostgresPassword { get; set; } = "";

        [JsonPropertyName("postgres_database")]
        public string PostgresDatabase { get; set; } = "modcore_next";

        [JsonPropertyName("postgres_host")]
        public string PostgresHost { get; set; } = "127.0.0.1";

        [JsonPropertyName("postgres_port")]
        public int PostgresPort { get; set; } = 5432;
    }
}
