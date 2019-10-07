using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Api
{
	public class DiscordBots
	{
        [Obsolete("Library not compatible with net core 3, dropping support.")]
        public DiscordBots(string dbltoken, ulong botid, bool enabled)
		{
		}

        [Obsolete("Library not compatible with net core 3, dropping support.")]
		public async Task UpdateGuildCount(int count)
		{
            await Task.Yield();
		}
	}
}
