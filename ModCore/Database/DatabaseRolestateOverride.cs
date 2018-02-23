namespace ModCore.Database
{
    public class DatabaseRolestateOverride
    {
        public int Id { get; set; }
        public long MemberId { get; set; }
        public long GuildId { get; set; }
        public long ChannelId { get; set; }
        public long? PermsAllow { get; set; }
        public long? PermsDeny { get; set; }
    }
}
