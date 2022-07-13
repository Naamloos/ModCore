using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ModCore.Utils.EntityFramework.AttributeImpl;

namespace ModCore.Database.DatabaseEntities
{
    [Table("mcore_levels")]
    public class DatabaseLevel
    {
        #region Index
        [Index("channel_id_user_id_key", IsUnique = true, IsLocal = true)]
        [Column("channel_id")]
        public long GuildId { get; set; }

        [Index("channel_id_user_id_key", IsUnique = true, IsLocal = true)]
        [Column("user_id")]
        public long UserId { get; set; }
        #endregion

        /// <summary>
        /// Ignore me. Indexed by guild/user.
        /// </summary>
        [Column("id")]
        public int Id { get; set; }

        [Column("experience")]
        public int Experience { get; set; }

        [Column("last_xp_grant", TypeName = "timestamptz")]
        public DateTime LastXpGrant { get; set; }
    }
}
