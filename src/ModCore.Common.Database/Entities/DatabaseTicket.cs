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
        [JsonPropertyName("id")]
        [Column("id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public long Id { get; set; }

        [JsonPropertyName("guild_id")]
        [Column("guild_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong GuildId { get; set; }

        [JsonPropertyName("author_id")]
        [Column("author_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong AuthorId { get; set; }

        [JsonPropertyName("name")]
        [Column("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        [Column("description")]
        public string Description { get; set; }

        [JsonPropertyName("ticket_thread_id")]
        [Column("ticket_thread_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong? TicketThreadId { get; set; }

        [JsonIgnore]
        public virtual DatabaseGuild Guild { get; set; }
        [JsonIgnore]
        public virtual DatabaseUser Author { get; set; }
    }
}
