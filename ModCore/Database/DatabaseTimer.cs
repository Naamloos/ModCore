using System;
using ModCore.Entities;
using Newtonsoft.Json;

namespace ModCore.Database
{
    public class DatabaseTimer
    {
        public int Id { get; set; }
        public long GuildId { get; set; }
        public long ChannelId { get; set; }
        public long UserId { get; set; }
        public DateTime DispatchAt { get; set; }
        public TimerActionType ActionType { get; set; }
        public string ActionData { get; set; }

        public T GetData<T>() where T : class, ITimerData =>
            JsonConvert.DeserializeObject<T>(this.ActionData);

        public void SetData<T>(T data) where T : class, ITimerData =>
            this.ActionData = JsonConvert.SerializeObject(data);
    }
}
