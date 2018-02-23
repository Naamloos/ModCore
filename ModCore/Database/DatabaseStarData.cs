namespace ModCore.Database
{
    public class DatabaseStarData
    {
        public int Id { get; set; }
        public long StargazerId { get; set; } // Member that starred

        public long StarboardMessageId { get; set; } // Id for starboard entry message
        public long MessageId { get; set; } // Message Id
        public long AuthorId { get; set; } // Author Id
        public long GuildId { get; set; } // Guild this belongs to
        public long ChannelId { get; set; } // Channel this was sent in
    }
}