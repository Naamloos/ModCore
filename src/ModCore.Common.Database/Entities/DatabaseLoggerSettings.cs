using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_logger_settings")]
    public class DatabaseLoggerSettings
    {
        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [Column("logger_channel_id")]
        public ulong LoggerChannelId { get; set; }

        [Column("log_joins")]
        public bool LogJoins { get; set; }

        [Column("log_message_edit")]
        public bool LogMessageEdits { get; set; }

        [Column("log_nicknames")]
        public bool LogNicknames { get; set; }

        [Column("log_avatars")]
        public bool LogAvatars { get; set; }

        [Column("log_invites")]
        public bool LogInvites { get; set; }

        [Column("log_role_assign")]
        public bool LogRoleAssignment { get; set; }

        [Column("log_channels")]
        public bool LogChannels { get; set; }

        [Column("log_guild_edit")]
        public bool LogGuildEdits { get; set; }

        [Column("log_role_edit")]
        public bool LogRoleEdits { get; set; }

        public virtual DatabaseGuild Guild { get; set; }
    }
}
