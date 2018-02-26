namespace ModCore.Database
{
    public class DatabaseModNote
    {
        public int Id { get; set; }
        public long MemberId { get; set; }
        public long GuildId { get; set; }
        public string Contents { get; set; }
    }
}
