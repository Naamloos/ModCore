using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_ticket")]
    public class DatabaseTicket
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }

        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [Column("author_id")]
        public ulong AuthorId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("ticket_thread_id")]
        public ulong? TicketThreadId { get; set; }

        public virtual DatabaseGuild Guild { get; set; }
        public virtual DatabaseUser Author { get; set; }
    }
}
