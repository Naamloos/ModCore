using DiscordBotsList.Api;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Api
{
	/// <summary>
	/// Wrapping multiple Discord Bot List apis, for easy access.
	/// </summary>
    public class BotLists
    {
		private AuthDiscordBotListApi _dbl;

		public BotLists(ulong id, string dbl_token)
		{
			_dbl = new AuthDiscordBotListApi(id, dbl_token);
		}

		public async Task UpdateGuildCountAsync(int guildcount)
		{
			var dbl_me = await _dbl.GetMeAsync();
			await dbl_me.UpdateStatsAsync(guildcount);
			// implement the other one too
		}
    }
}
