using System;

namespace ModCore.Database
{
    public partial class DatabaseWarning
    {
        public int Id { get; set; }
        public long MemberId { get; set; }
        public long GuildId { get; set; }
        public long IssuerId { get; set; }
        public DateTime IssuedAt { get; set; }
        public string WarningText { get; set; }
    }
}
