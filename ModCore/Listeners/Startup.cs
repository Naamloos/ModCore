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
            try
            {
                await e.Guild.GetChannel(366601285669224458).SendMessageAsync("TEST: " + bot.SharedData.StartNotify.guild + " - " + bot.SharedData.StartNotify.channel);
            }
            catch
            {

            }
            if (e.Guild.Id == bot.SharedData.StartNotify.guild)
            {
                await e.Guild.GetChannel(bot.SharedData.StartNotify.channel).SendMessageAsync("Heeey, VSauce here.");
            }
        }
    }
}
