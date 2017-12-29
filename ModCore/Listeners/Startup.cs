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
            if (e.Guild.Id == bot.SharedData.StartNotify.guild)
            {
                await e.Guild.GetChannel(bot.SharedData.StartNotify.channel).SendMessageAsync("Done updating. We're back online!\nFuck you SSG.");
            }
        }
    }
}
