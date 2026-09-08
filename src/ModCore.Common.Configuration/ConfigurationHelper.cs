using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ModCore.Common.Configuration
{
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Creates env file if it does not yet exist.
        /// </summary>
        /// <returns>true if the file already existed, false if it was created.</returns>
        public static bool CreateEnvFileIfNotExists()
        {
            string path = GetDefaultEnvPath();

            if (!File.Exists(path))
            {
                StringBuilder builder = new StringBuilder();
                Enum.GetValues<ConfigKey>().ToList().ForEach(key =>
                {
                    var description = key.GetAttributeOfType<DescriptionAttribute>()?.Description ?? "No description available.";
                    builder.AppendLine($"# {description}");
                    builder.AppendLine($"{GetConfigKeyString(key)}=");
                    builder.AppendLine();
                });

                File.Create(path).Close();
                File.WriteAllText(path, builder.ToString());
                return false;
            }

            return true;
        }

        public static string GetDefaultEnvPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env");
        }

        public static string GetConfigKeyString(ConfigKey key)
        {
            if(!Enum.IsDefined(typeof(ConfigKey), key))
                throw new ArgumentException($"Invalid config key: {key}", nameof(key));

            return ToSnakeCase(Enum.GetName<ConfigKey>(key)!);
        }

        private static TAttribute? GetAttributeOfType<TAttribute>(this Enum enumValue)
            where TAttribute : Attribute
        {
            var memberInfo = enumValue
                .GetType()
                .GetMember(enumValue.ToString())
                .FirstOrDefault();

            return memberInfo?
                .GetCustomAttribute<TAttribute>();
        }

        private static string ToSnakeCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var result = Regex.Replace(value, @"([a-z0-9])([A-Z])", "$1_$2");
            result = Regex.Replace(result, @"([A-Z]+)([A-Z][a-z])", "$1_$2");

            return result.ToLowerInvariant();
        }
    }

    /// <summary>
    /// Configuration keys that define what config option must be retrieved
    /// </summary>
    public enum ConfigKey
    {
        /// <summary>
        /// Token to authorize with the Discord API.
        /// </summary>
        [Description("Token to authorize with the Discord API.")]
        DiscordToken,

        /// <summary>
        /// Discord API Client ID
        /// </summary>
        [Description("Discord API Client ID")]
        ClientId,

        /// <summary>
        /// Discord API Client Secret
        /// </summary>
        [Description("Discord API Client Secret")]
        ClientSecret,

        /// <summary>
        /// Discord API Application ID
        /// </summary>
        [Description("Discord API Application ID")]
        ApplicationId,

        /// <summary>
        /// URL of the proxy to use for Discord REST API requests. If empty, requests will be sent directly to Discord.
        /// </summary>
        [Description("URL of the proxy to use for Discord REST API requests. If empty, requests will be sent directly to Discord.")]
        DiscordRestProxy,

        /// <summary>
        /// Amount of shards to run.
        /// </summary>
        [Description("Amount of shards to run.")]
        ShardCount,

        /// <summary>
        /// The current shard. This will be unique per shard instance, and is overridden on the container level. For debugging purposes, you want ID 0.
        /// </summary>
        [Description("The current shard. This will be unique per shard instance, and is overridden on the container level. For debugging purposes, you want ID 0.")]
        CurrentShard,

        /// <summary>
        /// Username to connect to the Postgres database with.
        /// </summary>
        [Description("Username to connect to the Postgres database with.")]
        PostgresUsername,

        /// <summary>
        /// Password to connect to the Postgres database with.
        /// </summary>
        [Description("Password to connect to the Postgres database with.")]
        PostgresPassword,

        /// <summary>
        /// Name of the Postgres database to connect to.
        /// </summary>
        [Description("Name of the Postgres database to connect to.")]
        PostgresDatabase,

        /// <summary>
        /// Hostname (or IP) of the Postgres database to connect to.
        /// </summary>
        [Description("Hostname (or IP) of the Postgres database to connect to.")]
        PostgresHost,

        /// <summary>
        /// Port of the Postgres database to connect to.
        /// </summary>
        [Description("Port of the Postgres database to connect to.")]
        PostgresPort,

        /// <summary>
        /// Master key used for encryption and decryption of some database values. 
        /// This should be a random string of exactly 32 characters and must NOT change after being set.
        /// Doing so will render most of your database useless!
        /// </summary>
        [Description("Master key used for encryption and decryption of some database values. This should be a random string of exactly 32 characters and must NOT change after being set. Doing so will render most of your database useless!")]
        MasterKey,

        /// <summary>
        /// Connection string for Redis cache. Not necessary in development, as cache will be in memory.
        /// </summary>
        [Description("Connection string for Redis cache. Not necessary in development, as cache will be in memory.")]
        RedisConnectionString,
    }
}
