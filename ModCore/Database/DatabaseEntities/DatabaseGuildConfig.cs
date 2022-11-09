using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ModCore.Database.JsonEntities;
using ModCore.Entities;
using ModCore.Utils.EntityFramework.AttributeImpl;
using Newtonsoft.Json;

namespace ModCore.Database.DatabaseEntities
{
    [Table("mcore_guild_config")]
    public class DatabaseGuildConfig
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("guild_id")]
        [Index("guild_id_key", IsUnique = true, IsLocal = true)]
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
