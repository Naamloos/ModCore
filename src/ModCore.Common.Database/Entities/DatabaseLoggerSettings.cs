using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_logger_settings")]
    public class DatabaseLoggerSettings
    {
        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [JsonPropertyName("logger_channel_id")]
        [Column("logger_channel_id")]
        public ulong? LoggerChannelId { get; set; } = null;

        [JsonPropertyName("log_joins")]
        [Column("log_joins")]
        public bool LogJoins { get; set; }

        [JsonPropertyName("log_message_edit")]
        [Column("log_message_edit")]
        public bool LogMessageEdits { get; set; }

        [JsonPropertyName("log_nicknames")]
        [Column("log_nicknames")]
        public bool LogNicknames { get; set; }

        [JsonPropertyName("log_avatars")]
        [Column("log_avatars")]
        public bool LogAvatars { get; set; }

        [JsonPropertyName("log_invites")]
        [Column("log_invites")]
        public bool LogInvites { get; set; }

        [JsonPropertyName("log_role_assign")]
        [Column("log_role_assign")]
        public bool LogRoleAssignment { get; set; }

        [JsonPropertyName("log_channels")]
        [Column("log_channels")]
        public bool LogChannels { get; set; }

        [JsonPropertyName("log_guild_edit")]
        [Column("log_guild_edit")]
        public bool LogGuildEdits { get; set; }

        [JsonPropertyName("log_role_edit")]
        [Column("log_role_edit")]
        public bool LogRoleEdits { get; set; }

        public virtual DatabaseGuild Guild { get; set; }
    }
}
