using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ModCore.Logic.EntityFramework.AttributeImpl;

namespace ModCore.Database.DatabaseEntities
{
    [Table("mcore_database_info")]
    public class DatabaseInfo
    {
        [Column("id")]
        public int Id { get; set; }

        [Index("meta_key_key", IsUnique = true, IsLocal = true)]
        [Column("meta_key")]
        [Required]
        public string MetaKey { get; set; }

        [Column("meta_value")]
        [Required]
        public string MetaValue { get; set; }
    }
}
