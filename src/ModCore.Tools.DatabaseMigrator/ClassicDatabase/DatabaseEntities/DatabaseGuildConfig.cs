using Microsoft.EntityFrameworkCore;
using ModCore.Tools.DatabaseMigrator.ClassicDatabase.JsonEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Tools.DatabaseMigrator.ClassicDatabase.DatabaseEntities
{
    [Table("mcore_guild_config")]
    public class DatabaseGuildConfig
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("guild_id")]
        public long GuildId { get; set; }

        [Column("settings", TypeName = "jsonb")]
        [Required]
        public string Settings { get; set; }

        public GuildSettings GetSettings() =>
            JsonConvert.DeserializeObject<GuildSettings>(Settings);

        public void SetSettings(GuildSettings settings) =>
            Settings = JsonConvert.SerializeObject(settings);
    }
}
