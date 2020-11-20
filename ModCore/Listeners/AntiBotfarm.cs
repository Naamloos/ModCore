using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using F23.StringSimilarity;
using Humanizer;
using ModCore.Entities;
using ModCore.Logic;
using System.Text;
using ModCore.Logic.Extensions;
using DSharpPlus.EventArgs;

namespace ModCore.Listeners
{
    public class AntiBotfarm
    {
        [AsyncListener(EventTypes.GuildAvailable)]
        public static async Task GuildAdded(ModCoreShard bot, GuildCreateEventArgs args)
        {
            // ModCore checks whether a guild has:
            // - more than 15 bots
            // - more bots than users
            // - not whitelisted
        }
    }
}
