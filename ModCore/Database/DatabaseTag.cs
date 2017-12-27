using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Database
{
    public partial class DatabaseTag
    {
        public int Id { get; set; }
        public long ChannelId { get; set; }
        public long OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }
    }
}
