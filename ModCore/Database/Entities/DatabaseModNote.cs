using System.ComponentModel.DataAnnotations.Schema;
using ModCore.Logic.EntityFramework.AttributeImpl;

namespace ModCore.Database.Entities
{
    [Table("mcore_modnotes")]
    public class DatabaseModNote
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

        [Column("contents")]
        public string Contents { get; set; }
    }
}
