using System.Threading.Tasks;
using DSharpPlus.EventArgs;
using ModCore.Logic;

namespace ModCore.Listeners
{
    public static class Startup
    {
        [AsyncListener(EventTypes.GuildAvailable)]
        public static async Task UpdateCompleteAsync(ModCoreShard bot, GuildCreateEventArgs e)
        {
            if (e.Guild.Id == bot.SharedData.StartNotify.guild)
            {
                await e.Guild.GetChannel(bot.SharedData.StartNotify.channel).ElevatedMessageAsync("Done updating. We're back online!");
                bot.SharedData.StartNotify = default;
            }
        }
    }
}
