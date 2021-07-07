using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Entities
{
    [Table("guild_config")]
    public class GuildConfig
    {
        [Column("guild_id")]
        [Key]
        [Required]
        public long GuildId { get; set; }

        [Column("prefix")]
        public string Prefix { get; set; }

        [Column("mute_role_id")]
        public long MuteRoleId { get; set; }

        [Column("spelling_helper_enabled")]
        public bool SpellingHelperEnabled { get; set; }

        [Column("log_channel_id")]
        public long LogChannelId { get; set; }
    }
}
