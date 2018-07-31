using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModCore.Database
{
    [Table("mcore_warnings")]
    public class DatabaseWarning
    {
        [Column("id")]
        public int Id { get; set; }
        
        [Column("member_id")]
        public long MemberId { get; set; }
        
        [Column("guild_id")]
        public long GuildId { get; set; }
        
        [Column("issuer_id")]
        public long IssuerId { get; set; }
        
        [Column("issued_at", TypeName = "timestamptz")]
        public DateTime IssuedAt { get; set; }
        
        [Column("warning_text")]
        [Required]
        public string WarningText { get; set; }
    }
}
