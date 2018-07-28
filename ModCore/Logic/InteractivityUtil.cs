using DSharpPlus.Interactivity;
using ModCore.Logic.Extensions;

namespace ModCore.Logic
{
    public static class InteractivityUtil
    {
        public static bool Confirm(MessageContext m)
        {
            return m.Message.Content.EqualsIgnoreCase("yes")
                   || m.Message.Content.EqualsIgnoreCase("y")
                   || m.Message.Content.EqualsIgnoreCase("1")
                   || m.Message.Content.EqualsIgnoreCase("ya")
                   || m.Message.Content.EqualsIgnoreCase("ja")
                   || m.Message.Content.EqualsIgnoreCase("si")
                   || m.Message.Content.EqualsIgnoreCase("da");
        }
    }
}