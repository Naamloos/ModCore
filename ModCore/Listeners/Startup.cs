using DSharpPlus.Entities;
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
                DiscordChannel x = await e.Client.GetChannelAsync(366601285669224458);
                await x.SendMessageAsync("TEST: " + bot.SharedData.StartNotify.guild + " - " + bot.SharedData.StartNotify.channel);

            }
            catch
            {
                DiscordChannel x = await e.Client.GetChannelAsync(366601285669224458);
                await x.SendMessageAsync("TEST");
            }
            if (e.Guild.Id == bot.SharedData.StartNotify.guild)
            {
                DiscordChannel x = await e.Client.GetChannelAsync(bot.SharedData.StartNotify.channel);
                await x.SendMessageAsync("Heeey, VSauce here.");
            }
        }
    }
}
