using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Tools.DatabaseMigrator.ClassicDatabase.DatabaseEntities
{
    [Table("mcore_cmd_state")]
    public class DatabaseCommandId
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short Id { get; set; }

        [Key]
        [Column("command_qualified")]
        public string Command { get; set; }
    }
}
