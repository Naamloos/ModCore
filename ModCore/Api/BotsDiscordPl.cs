using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Api
{
	// that's literally
	public class BotsDiscordPl
	{
		private HttpClient _http;
		private int _shard_count;
		private ulong _bot_id;
		private bool _enabled;

		public BotsDiscordPl(int shard_count, string token, ulong botid, bool enabled)
		{
			_http = new HttpClient();
			_shard_count = shard_count;
			_bot_id = botid;
			_enabled = enabled;
		}

		public async Task UpdateShardAsync(int id, int count)
		{
			if (_enabled)
			{
				// am very lazy sorry it's late but this should just work
				JObject payload = new JObject();
				payload.Add("shard_id", id);
				payload.Add("shard_count", _shard_count);
				payload.Add("guild_count", count);

				var content = new StringContent(payload.ToString(), Encoding.UTF8, "application/json");

				await _http.PostAsync($"https://bots.discord.pw/api/bots/{_bot_id}/stats", content);
			}
		}
	}
}
