using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ModCore.Utils.EntityFramework.AttributeImpl;

namespace ModCore.Database.DatabaseEntities
{
    [Table("mcore_rolestate_overrides")]
    public class DatabaseRolestateOverride
    {
        #region Index
        [Index("member_id_guild_id_channel_id_key", IsUnique = true, IsLocal = true)]
        [Column("member_id")]
        public long MemberId { get; set; }

        [Index("member_id_guild_id_channel_id_key", IsUnique = true, IsLocal = true)]
        [Column("guild_id")]
        public long GuildId { get; set; }

        [Index("member_id_guild_id_channel_id_key", IsUnique = true, IsLocal = true)]
        [Column("channel_id")]
        public long ChannelId { get; set; }
        #endregion

        [Column("id")]
        public int Id { get; set; }

        [Column("perms_allow")]
        public long? PermsAllow { get; set; }

        [Column("perms_deny")]
        public long? PermsDeny { get; set; }
    }
}
