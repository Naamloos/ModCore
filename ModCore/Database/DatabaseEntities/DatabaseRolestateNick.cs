using System;
using System.ComponentModel.DataAnnotations.Schema;
using ModCore.Entities;
using ModCore.Utils.EntityFramework.AttributeImpl;

namespace ModCore.Database.DatabaseEntities
{
    [Table("mcore_rolestate_nicks")]
    public class DatabaseRolestateNick
    {
        #region Index
        [Index("member_id_guild_id_key", IsUnique = true, IsLocal = true)]
        [Column("member_id")]
        public long MemberId { get; set; }

        [Index("member_id_guild_id_key", IsUnique = true, IsLocal = true)]
        [Column("guild_id")]
        public long GuildId { get; set; }
        #endregion

        [Column("id")]
        public int Id { get; set; }

        [Column("nickname")]
        public string Nickname { get; set; }
    }
}
