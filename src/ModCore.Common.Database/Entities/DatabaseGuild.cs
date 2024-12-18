using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_guild")]
    public class DatabaseGuild
    {
        [JsonIgnore]
        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [JsonPropertyName("logging_channel_id")]
        [Column("logging_channel_id")]
        public ulong? LoggingChannelId { get; set; } = null;

        [JsonPropertyName("modlog_channel_id")]
        [Column("modlog_channel_id")]
        public ulong? ModlogChannelId { get; set; } = null;

        [JsonPropertyName("ticket_channel_id")]
        [Column("ticket_channel_id")]
        public ulong? TicketChannelId { get; set; } = null;

        [JsonPropertyName("appeal_channel_id")]
        [Column("appeal_channel_id")]
        public ulong? AppealChannelId { get; set; } = null;

        /// <summary>
        /// 0 if disabled
        /// </summary>
        [JsonPropertyName("nick_confirm_channel_id")]
        [Column("nick_confirm_channel_id")]
        public ulong? NicknameConfirmationChannelId { get; set; } = 0;

        [JsonPropertyName("last_seen_at")]
        [Column("last_seen_at")]
        public DateTimeOffset? LastSeenAt { get; set; } = DateTimeOffset.UtcNow;

        [JsonPropertyName("auto_role_enabled")]
        [Column("auto_role_enabled")]
        public bool AutoRoleEnabled { get; set; } = false;

        [JsonPropertyName("embed_message_links_state")]
        [Column("embed_message_links_state")]
        public EmbedMessageLinks EmbedMessageLinks { get; set; }

        [JsonPropertyName("persist_user_roles")]
        [Column("persist_user_roles")]
        public bool PersistUserRoles { get; set; } = false;

        [JsonPropertyName("persist_user_overrides")]
        [Column("persist_user_overrides")]
        public bool PersistUserOverrides { get; set; } = false;

        [JsonPropertyName("persist_user_nicknames")]
        [Column("persist_user_nicknames")]
        public bool PersistUserNicknames { get; set; } = false;

        // Having everything referencing the base guild means we can easily delete one guild's
        // data by cascading everything. ModCore will have a data retention period of 1 year.
        [JsonIgnore]
        public virtual ICollection<DatabaseLevelData> LevelData { get; set; } = new HashSet<DatabaseLevelData>();
        [JsonPropertyName("starboards")]
        public virtual ICollection<DatabaseStarboard> Starboards { get; set; } = new HashSet<DatabaseStarboard>();
        [JsonIgnore]
        public virtual ICollection<DatabaseTag> Tags { get; set; } = new HashSet<DatabaseTag>();
        [JsonIgnore]
        public virtual ICollection<DatabaseNicknameState> NicknameStates { get; set; } = new HashSet<DatabaseNicknameState>();
        [JsonIgnore]
        public virtual ICollection<DatabaseRoleState> RoleStates { get; set; } = new HashSet<DatabaseRoleState>();
        [JsonIgnore]
        public virtual ICollection<DatabaseOverrideState> OverrideStates { get; set; } = new HashSet<DatabaseOverrideState>();
        [JsonPropertyName("auto_roles")]
        public virtual ICollection<DatabaseAutoRole> AutoRoles { get; set; } = new HashSet<DatabaseAutoRole>();
        [JsonIgnore]
        public virtual ICollection<DatabaseInfraction> Infractions { get; set; } = new HashSet<DatabaseInfraction>();
        [JsonIgnore]
        public virtual ICollection<DatabaseBanAppeal> BanAppeals { get; set; } = new HashSet<DatabaseBanAppeal>();
        [JsonIgnore]
        public virtual ICollection<DatabaseTicket> Tickets { get; set; } = new HashSet<DatabaseTicket>();
        [JsonPropertyName("role_menus")]
        public virtual ICollection<DatabaseRoleMenu> RoleMenus { get; set; } = new HashSet<DatabaseRoleMenu>();

        /// <summary>
        /// When this is null / doesn't exist, Logger is disabled.
        /// </summary>
        [JsonPropertyName("logger_settings")]
        public virtual DatabaseLoggerSettings LoggerSettings { get; set; } = new DatabaseLoggerSettings();

        /// <summary>
        /// When this is null / doesn't exist, Welcomer is disabled.
        /// </summary>
        [JsonPropertyName("welcome_settings")]
        public virtual DatabaseWelcomeSettings WelcomeSettings { get; set; } = new DatabaseWelcomeSettings();

        [JsonPropertyName("level_settings")]
        public virtual DatabaseLevelSettings LevelSettings { get; set; } = new DatabaseLevelSettings();
    }

    public enum EmbedMessageLinks
    {
        Disabled = 0,
        Prefixed = 1,
        Always = 2
    }
}
