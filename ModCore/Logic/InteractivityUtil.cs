using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using ModCore.Logic.Extensions;

namespace ModCore.Logic
{
    public static class InteractivityUtil
    {
        public static bool Confirm(DiscordMessage m)
        {
            return m.Content.EqualsIgnoreCase("yes")
                   || m.Content.EqualsIgnoreCase("y")
                   || m.Content.EqualsIgnoreCase("1")
                   || m.Content.EqualsIgnoreCase("ya")
                   || m.Content.EqualsIgnoreCase("ja")
                   || m.Content.EqualsIgnoreCase("si")
                   || m.Content.EqualsIgnoreCase("da");
        }
    }
}