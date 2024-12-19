using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_ticket")]
    public class DatabaseTicket
    {
        [JsonIgnore]
        [Column("id")]
        public long Id { get; set; }

        [JsonIgnore]
        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [JsonPropertyName("author_id")]
        [Column("author_id")]
        public ulong AuthorId { get; set; }

        [JsonPropertyName("name")]
        [Column("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        [Column("description")]
        public string Description { get; set; }

        [JsonPropertyName("ticket_thread_id")]
        [Column("ticket_thread_id")]
        public ulong? TicketThreadId { get; set; }

        [JsonIgnore]
        public virtual DatabaseGuild Guild { get; set; }
        [JsonIgnore]
        public virtual DatabaseUser Author { get; set; }
    }
}
