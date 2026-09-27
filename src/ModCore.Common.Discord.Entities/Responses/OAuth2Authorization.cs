namespace ModCore.Common.Discord.Entities.Responses
{
    public record OAuth2Authorization
    {
        public Interactions.Application Application { get; set; } = default!;
        public string[] Scopes { get; set; } = [];
        public DateTimeOffset Expires { get; set; }
        public Optional<User> User { get; set; }
    }
}
