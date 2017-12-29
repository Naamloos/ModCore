using DSharpPlus.EventArgs;
using ModCore.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Listeners
{
    public static class Startup
    {
        [AsyncListener(EventTypes.GuildAvailable)]
        public static async Task UpdateCompleteAsync(ModCoreShard bot, GuildCreateEventArgs e)
        {
            if (bot.SharedData.StartNotify.guild == e.Guild.Id)
            {
                await e.Guild.GetChannel(bot.SharedData.StartNotify.channel).SendMessageAsync("Updated!");
            }
        }
    }
}
