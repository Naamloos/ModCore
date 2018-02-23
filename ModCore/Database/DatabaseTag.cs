using System;

namespace ModCore.Database
{
    public class DatabaseTag
    {
        public int Id { get; set; }
        public long ChannelId { get; set; }
        public long OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }
    }
}
