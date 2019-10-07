using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DiscordBotsList.Api;
using DiscordBotsList.Api.Objects;

namespace ModCore.Api
{
	public class DiscordBots
	{
		private AuthDiscordBotListApi dblapi;
		private IDblSelfBot me = null;
		private bool _enabled;

		public DiscordBots(string dbltoken, ulong botid, bool enabled)
		{
			dblapi = new AuthDiscordBotListApi(botid, dbltoken);
			_enabled = enabled;
		}

		public async Task UpdateGuildCount(int count)
		{
			if (_enabled)
			{
				if (me == null)
					this.me = await dblapi.GetMeAsync();

				await me.UpdateStatsAsync(count);
			}
		}
	}
}
