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
            // No.
            await Task.Yield();
		}
	}
}
