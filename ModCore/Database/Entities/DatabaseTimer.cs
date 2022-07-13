using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ModCore.Entities;
using Newtonsoft.Json;

namespace ModCore.Database.Entities
{
    [Table("mcore_timers")]
    public class DatabaseTimer
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("guild_id")]
        public long GuildId { get; set; }

        [Column("channel_id")]
        public long ChannelId { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [Column("dispatch_at", TypeName = "timestamptz")]
        public DateTime DispatchAt { get; set; }

        [Column("action_type")]
        public TimerActionType ActionType { get; set; }

        [Column("action_data", TypeName = "jsonb")]
        [Required]
        public string ActionData { get; set; }

        public T GetData<T>() where T : class, ITimerData =>
            JsonConvert.DeserializeObject<T>(ActionData);

        public void SetData<T>(T data) where T : class, ITimerData =>
            ActionData = JsonConvert.SerializeObject(data);
    }
}
