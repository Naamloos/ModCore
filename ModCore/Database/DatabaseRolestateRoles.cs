namespace ModCore.Database
{
    public partial class DatabaseRolestateRoles
    {
        public int Id { get; set; }
        public long MemberId { get; set; }
        public long GuildId { get; set; }
        public long[] RoleIds { get; set; }
    }
}
