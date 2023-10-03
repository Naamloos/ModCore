using ModCore.Common.Discord.Entities.Enums;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record Role
    {
        public Snowflake Id { get; set; }

        public string Name { get; set; }

        public int Color { get; set; }

        public bool Hoist { get; set; }

        public Optional<string?> Icon { get; set; }

        public Optional<string?> UnicodeEmoji { get; set; }

        public int Position { get; set; }

        public string Permissions { get; set; }

        public bool Managed { get; set; }

        public bool Mentionable { get; set; }

        public Optional<RoleTag[]> Tags { get; set; }

        public RoleFlags Flags { get; set; }
    }
}