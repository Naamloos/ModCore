using ModCore.Common.Database.Timers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_timers")]
    public class DatabaseTimer
    {
        [Column("timer_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong TimerId { get; set; }

        [Column("guild_id")]
        public ulong GuildId { get; set; }
        
        [Column("trigger_at")]
        public DateTimeOffset TriggersAt { get; set; }

        [Column("type")]
        public TimerTypes Type { get; set; }

        [Column("data", TypeName = "jsonb")]
        public string? Data { get; set; }

        public T GetData<T>() where T : class, ITimerData 
            => JsonSerializer.Deserialize<T>(Data);

        public void SetData<T>(T data) where T : class, ITimerData 
            => Data = JsonSerializer.Serialize(data);
    }
}
