namespace ModCore.Database
{
    public partial class DatabaseModNote
    {
        public int Id { get; set; }
        public long MemberId { get; set; }
        public long GuildId { get; set; }
        public string Contents { get; set; }
    }
}
