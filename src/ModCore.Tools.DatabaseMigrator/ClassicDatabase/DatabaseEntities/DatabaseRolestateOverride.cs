﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Tools.DatabaseMigrator.ClassicDatabase.DatabaseEntities
{
    [Table("mcore_rolestate_overrides")]
    public class DatabaseRolestateOverride
    {
        #region Index
        [Column("member_id")]
        public long MemberId { get; set; }

        [Column("guild_id")]
        public long GuildId { get; set; }

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
