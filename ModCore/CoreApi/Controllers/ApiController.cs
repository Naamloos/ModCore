using Microsoft.AspNetCore.Mvc;
using ModCore.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModCore.CoreApi.Controllers
{
	[Route("{token}")]
	public class ApiController : Controller
    {
		private ModCore core;

		public ApiController(CoreContainer cont)
		{
			this.core = cont.mcore;
		}

		public string Index(string token)
		{
			if(string.IsNullOrEmpty(token) || token != core.SharedData.Token)
			{
				NotFound();
				return null;
			}

			return "ModCore API";
		}

		// GET ping
		[HttpGet("ping")]
		public string Ping(string token)
		{
			if (string.IsNullOrEmpty(token) || token != core.SharedData.Token)
			{
				NotFound();
				return null;
			}

			return "pong!";
		}

		[HttpGet("default")]
		public string Dp(string token)
		{
			if (string.IsNullOrEmpty(token) || token != core.SharedData.Token)
			{
				NotFound();
				return null;
			}

			return "Default prefix: " + core.SharedData.DefaultPrefix;
		}

		[HttpGet("config/{gid}")]
		public string Config(string token, long gid)
		{
			if (string.IsNullOrEmpty(token) || token != core.SharedData.Token)
			{
				NotFound();
				return null;
			}

			var gcs = this.core.Shards.SelectMany(x => x.Client.Guilds.Values);
			if (gcs.Any(x => x.Id == (ulong)gid))
			{
				var gd = gcs.First(x => x.Id == (ulong)gid);
				var cfg = gd.GetGuildSettings(this.core.Shards.First().Database.CreateContext());
				return JsonConvert.SerializeObject(cfg);
			}

			this.NotFound();
			return null;
		}
	}
}
