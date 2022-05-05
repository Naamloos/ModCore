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

        [JsonProperty("use_perspective")]
        public bool UsePerspective { get; private set; }

        [JsonProperty("perspective_token")]
        internal string PerspectiveToken { get; private set; }

        [JsonProperty("database")]
        internal DatabaseSettings Database { get; private set; }

		[JsonProperty("bot_managers")]
		public List<ulong> BotManagers { get; private set; } = new List<ulong>();

		[JsonProperty("bot_id")]
		public ulong BotId { get; private set; } = 359828546719449109;

		[JsonProperty("dbl_token")]
		public string DblToken { get; private set; }

		[JsonProperty("bot_discord_pl_token")]
		public string BotDiscordPlToken { get; private set; }

		[JsonProperty("bot_lists_enabled")]
        [Obsolete("I just use the separate enablers now.")]
		public bool BotListsEnabled { get; private set; } = false;

		[JsonProperty("botlist_pw_enable")]
		public bool BotListPwEnable { get; private set; } = false;

		[JsonProperty("botlist_org_enable")]
		public bool BotListOrgEnable { get; private set; } = false;

        [JsonProperty("discordbotlist_com_enable")]
        public bool DiscordBotListComEnable { get; private set; } = false;

        [JsonProperty("discordbotlist_com_token")]
        public string DiscordBotListComToken { get; private set; }

        [JsonProperty("botsondiscord_xyz_enable")]
        public bool BotsOnDiscordXyzEnable { get; private set; } = false;

        [JsonProperty("botsondiscord_xyz_token")]
        public string BotsOnDiscordXyzToken { get; private set; }
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
