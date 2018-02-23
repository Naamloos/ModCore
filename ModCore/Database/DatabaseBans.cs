using System;

namespace ModCore.Database
{
    public class DatabaseBan
    {
        public int Id { get; set; }
        public long GuildId { get; set; }
        public long UserId { get; set; }
        public DateTime IssuedAt { get; set; }
        public string BanReason { get; set; }
    }
}