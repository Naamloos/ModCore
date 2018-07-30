using System.ComponentModel.DataAnnotations.Schema;

namespace ModCore.Database
{
    [Table("mcore_database_info")]
    public class DatabaseInfo
    {
        public int Id { get; set; }
        public string MetaKey { get; set; }
        public string MetaValue { get; set; }
    }
}
