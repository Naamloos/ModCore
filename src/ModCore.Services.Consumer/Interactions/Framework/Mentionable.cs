using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Guilds;

namespace ModCore.Services.Consumer.Interactions.Framework
{
    public sealed class Mentionable
    {
        public Snowflake Id { get; }

        public User? User { get; }

        public Member? Member { get; }

        public Role? Role { get; }

        public bool IsUser => User is not null;

        public bool IsMember => Member is not null;

        public bool IsRole => Role is not null;

        private Mentionable(
            Snowflake id,
            User? user,
            Member? member,
            Role? role)
        {
            Id = id;
            User = user;
            Member = member;
            Role = role;
        }

        public static Mentionable FromUser(
            Snowflake id,
            User user,
            Member? member = null)
        {
            return new Mentionable(id, user, member, null);
        }

        public static Mentionable FromRole(Snowflake id, Role role)
        {
            return new Mentionable(id, null, null, role);
        }

        public string Mention()
        {
            if (this.IsUser || this.IsMember)
            {
                return $"<@{Id}>";

            }
            else if (this.IsRole)
            {
                return $"<@&{Id}>";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}