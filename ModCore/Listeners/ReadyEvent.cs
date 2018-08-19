using DSharpPlus.EventArgs;
using ModCore.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Listeners
{
    public class ReadyEvent
    {
		[AsyncListener(EventTypes.Ready)]
		public static async Task BotListUpdate(ModCoreShard bot, ReadyEventArgs e)
		{
			await bot.SharedData.BotsDiscordPl.UpdateShardAsync(bot.ShardId, e.Client.Guilds.Count);
			bot.SharedData.ReadysReceived++;
			if (bot.SharedData.ReadysReceived == bot.SharedData.ModCore.Shards.Count)
				await bot.SharedData.DiscordBots.UpdateGuildCount(bot.SharedData.ModCore.Shards.Select(x => x.Client.Guilds.Count).Sum());
		}
	}
}
