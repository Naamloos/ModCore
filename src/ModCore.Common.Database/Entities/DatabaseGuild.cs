using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_guild")]
    public class DatabaseGuild
    {
        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [Column("logging_channel_id")]
        public ulong? LoggingChannelId { get; set; } = null;

        [Column("modlog_channel_id")]
        public ulong? ModlogChannelId { get; set; } = null;

        [Column("ticket_channel_id")]
        public ulong? TicketChannelId { get; set; } = null;

        [Column("appeal_channel_id")]
        public ulong? AppealChannelId { get; set; } = null;

        /// <summary>
        /// 0 if disabled
        /// </summary>
        [Column("nick_confirm_channel_id")]
        public ulong? NicknameConfirmationChannelId { get; set; } = 0;

        [Column("last_seen_at")]
        public DateTimeOffset? LastSeenAt { get; set; } = DateTimeOffset.UtcNow;

        [Column("auto_role_enabled")]
        public bool AutoRoleEnabled { get; set; } = false;

        [Column("embed_message_links_state")]
        public EmbedMessageLinks EmbedMessageLinks { get; set; }

        [Column("persist_user_roles")]
        public bool PersistUserRoles { get; set; } = false;

        [Column("persist_user_overrides")]
        public bool PersistUserOverrides { get; set; } = false;

        [Column("persist_user_nicknames")]
        public bool PersistUserNicknames { get; set; } = false;

        // Having everything referencing the base guild means we can easily delete one guild's
        // data by cascading everything. ModCore will have a data retention period of 1 year.
        public virtual ICollection<DatabaseLevelData> LevelData { get; set; } = new HashSet<DatabaseLevelData>();
        public virtual ICollection<DatabaseStarboard> Starboards { get; set; } = new HashSet<DatabaseStarboard>();
        public virtual ICollection<DatabaseTag> Tags { get; set; } = new HashSet<DatabaseTag>();
        public virtual ICollection<DatabaseNicknameState> NicknameStates { get; set; } = new HashSet<DatabaseNicknameState>();
        public virtual ICollection<DatabaseRoleState> RoleStates { get; set; } = new HashSet<DatabaseRoleState>();
        public virtual ICollection<DatabaseOverrideState> OverrideStates { get; set; } = new HashSet<DatabaseOverrideState>();
        public virtual ICollection<DatabaseAutoRole> AutoRoles { get; set; } = new HashSet<DatabaseAutoRole>();
        public virtual ICollection<DatabaseInfraction> Infractions { get; set; } = new HashSet<DatabaseInfraction>();
        public virtual ICollection<DatabaseBanAppeal> BanAppeals { get; set; } = new HashSet<DatabaseBanAppeal>();
        public virtual ICollection<DatabaseTicket> Tickets { get; set; } = new HashSet<DatabaseTicket>();
        public virtual ICollection<DatabaseRoleMenu> RoleMenus { get; set; } = new HashSet<DatabaseRoleMenu>();

        /// <summary>
        /// When this is null / doesn't exist, Logger is disabled.
        /// </summary>
        public virtual DatabaseLoggerSettings LoggerSettings { get; set; }

        /// <summary>
        /// When this is null / doesn't exist, Welcomer is disabled.
        /// </summary>
        public virtual DatabaseWelcomeSettings WelcomeSettings { get; set; }

        public virtual DatabaseLevelSettings LevelSettings { get; set; }
    }

    public enum EmbedMessageLinks
    {
        Disabled = 0,
        Prefixed = 1,
        Always = 2
    }
}
