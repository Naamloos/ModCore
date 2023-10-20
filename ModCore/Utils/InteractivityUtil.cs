﻿using DSharpPlus.Entities;
using ModCore.Utils.Extensions;

namespace ModCore.Utils
{
    public static class InteractivityUtil
    {
        public static bool Confirm(DiscordMessage message)
        {
            return message.Content.EqualsIgnoreCase("yes")
                   || message.Content.EqualsIgnoreCase("y")
                   || message.Content.EqualsIgnoreCase("1")
                   || message.Content.EqualsIgnoreCase("ya")
                   || message.Content.EqualsIgnoreCase("ja")
                   || message.Content.EqualsIgnoreCase("si")
                   || message.Content.EqualsIgnoreCase("da");
        }
    }
}