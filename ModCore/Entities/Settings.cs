using System;
using System.Collections.Generic;
using ModCore.Database;
using Newtonsoft.Json;
using Npgsql;

namespace ModCore.Entities
{
    public class Settings
    {
        [JsonProperty("token")]
        internal string Token { get; private set; }

        [JsonProperty("prefix")]
        public string DefaultPrefix { get; private set; }

        [JsonProperty("shard_count")]
        public int ShardCount { get; private set; }

        [JsonProperty("database")]
        internal DatabaseSettings Database { get; private set; }

		[JsonProperty("bot_id")]
		public ulong BotId { get; private set; } = 359828546719449109;

        [JsonProperty("contact_channel_id")]
        public ulong ContactChannelId { get; private set; } = 998624821653033051;

        [JsonProperty("sus_guild_channel_id")]
        public ulong SusGuildChannelId { get; private set; } = 1040374549293838376;
    }

    public struct DatabaseSettings
    {
        [JsonProperty("provider")]
        public DatabaseProvider Provider { get; private set; }

        /// <summary>
        /// Allows deserializing obsolete UseInMemoryProvider setting, but not serialization.
        /// </summary>
        [JsonProperty("in_memory")]
        [Obsolete("Use " + nameof(Provider) + " instead!")]
        public bool UseInMemoryProvider
        {
            // no getter!
            set { if (value) Provider = DatabaseProvider.InMemory; }
        }

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
        
        [JsonProperty("data_source")]
        public string DataSource { get; private set; }

        public string BuildConnectionString()
        {
            switch (this.Provider)
            {
                case DatabaseProvider.InMemory:
                    return null;
                case DatabaseProvider.Sqlite:
                    return "Data Source=" + this.DataSource;
                default:
                    return new NpgsqlConnectionStringBuilder
                    {
                        Host = this.Hostname,
                        Port = this.Port,
                        Database = this.Database,
                        Username = this.Username,
                        Password = this.Password,

                        SslMode = SslMode.Prefer,
                        TrustServerCertificate = true,

                        Pooling = true
                    }.ConnectionString;
            }

        }

        public DatabaseContextBuilder CreateContextBuilder() =>
            new DatabaseContextBuilder(this.Provider, this.BuildConnectionString());
    }

    public enum DatabaseProvider : byte
    {
        PostgreSql,
        InMemory,
        Sqlite
    }
}
