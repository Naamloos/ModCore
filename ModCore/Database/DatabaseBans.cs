using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModCore.Database
{
    [Table("mcore_bans")]
    public class DatabaseBan
    {
        [Column("id")]
        public int Id { get; set; }
        
        [Column("guild_id")]
        public long GuildId { get; set; }
        
        [Column("user_id")]
        public long UserId { get; set; }
        
        [Column("issued_at", TypeName = "timestamptz")]
        public DateTime IssuedAt { get; set; }
        
        [Column("ban_reason")]
        public string BanReason { get; set; }
    }
}