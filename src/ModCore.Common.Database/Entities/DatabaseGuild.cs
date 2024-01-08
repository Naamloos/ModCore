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
        public ulong? LoggingChannelId {  get; set; }

        [Column("modlog_channel_id")]
        public ulong? ModlogChannelId { get; set; }

        [Column("ticket_channel_id")]
        public ulong? TicketChannelId { get; set; }

        [Column("appeal_channel_id")]
        public ulong? AppealChannelId { get; set; }

        [Column("nick_confirm_channel_id")]
        public ulong? NicknameConfirmationChannelId { get; set; }

        // Having everything referencing the base guild means we can easily delete one guild's
        // data by cascading everything. ModCore will have a data retention period of 1 year.
        public virtual ICollection<DatabaseLevelData> LevelData { get; set; }
        public virtual ICollection<DatabaseStarboard> Starboards { get; set; }
        public virtual ICollection<DatabaseTag> Tags { get; set; }
        public virtual ICollection<DatabaseNicknameState> NicknameStates { get; set; }
        public virtual ICollection<DatabaseRoleState> RoleStates { get; set; }
        public virtual ICollection<DatabaseOverrideState> OverrideStates { get; set; }
        public virtual ICollection<DatabaseAutoRole> AutoRoles { get; set; }
        public virtual ICollection<DatabaseInfraction> Infractions { get; set; }
        public virtual ICollection<DatabaseBanAppeal> BanAppeals { get; set; }
        public virtual ICollection<DatabaseTicket> Tickets { get; set; }
        public virtual ICollection<DatabaseRoleMenu> RoleMenus { get; set; }

        /// <summary>
        /// When this is null / doesn't exist, Logger is disabled.
        /// </summary>
        public virtual DatabaseLoggerSettings LoggerSettings { get; set; }

        /// <summary>
        /// When this is null / doesn't exist, Welcomer is disabled.
        /// </summary>
        public virtual DatabaseWelcomeSettings WelcomeSettings { get; set; }
    }
}
