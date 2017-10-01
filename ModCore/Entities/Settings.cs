using ModCore.Database;
using Newtonsoft.Json;
using Npgsql;

namespace ModCore.Entities
{
    public struct Settings
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("prefix")]
        public string DefaultPrefix { get; private set; }

        [JsonProperty("shard_count")]
        public int ShardCount { get; private set; }

        [JsonProperty("database")]
        public DatabaseSettings Database { get; private set; }
    }

    public struct DatabaseSettings
    {
        [JsonProperty("hostname")]
        public string Hostname { get; private set; }

        [JsonProperty("port")]
        public int Port { get; private set; }

        [JsonProperty("database")]
        public string Database { get; private set; }

        [JsonProperty("username")]
        public string Username { get; private set; }

        [JsonProperty("password")]
        public string Password { get; private set; }

        public string BuildConnectionString()
        {
            var csb = new NpgsqlConnectionStringBuilder
            {
                Host = this.Hostname,
                Port = this.Port,
                Database = this.Database,
                Username = this.Username,
                Password = this.Password,

                SslMode = SslMode.Require,
                TrustServerCertificate = true,

                Pooling = false
            };

            return csb.ConnectionString;
        }

        public DatabaseContextBuilder CreateContextBuilder() =>
            new DatabaseContextBuilder(this.BuildConnectionString());
    }
}
