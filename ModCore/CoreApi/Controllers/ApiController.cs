using Microsoft.AspNetCore.Mvc;
using ModCore.CoreApi.Entities;
using ModCore.Database;
using ModCore.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModCore.CoreApi.Controllers
{
	[Route("api")]
	public class ApiController : Controller
    {
		private ModCore core;

		public ApiController(CoreContainer cont)
		{
			this.core = cont.mcore;
		}

		public string Index()
		{
			return "ModCore API";
		}

		[HttpGet("token/{token}")]
		public bool Token(string token)
		{
			// allow cross-domain shit for w/e reason
			Response.Headers.Add("Access-Control-Allow-Origin", "*");

			return core.SharedData.ApiToken == token;
		}

		// GET ping
		[HttpGet("ping")]
		public string Ping()
		{
			return "pong!";
		}

		[HttpGet("default")]
		public string Dp()
		{
			return "Default prefix: " + core.SharedData.DefaultPrefix;
		}

		[HttpGet("roles/{gid}")]
		public string Roles(ulong gid)
		{
			// Check token.
			var h = Request.Headers["Token"];
			if (string.IsNullOrEmpty(h) || core.SharedData.ApiToken != h)
			{
				this.Unauthorized();
				return null;
			}

			var arl = new ApiRoleList();
			var guildsfound = core.Shards.SelectMany(x => x.Client.Guilds.Values.Where(g => g.Id == gid)).ToArray();
			if (guildsfound.Length > 0)
			{
				foreach (var r in guildsfound[0].Roles)
				{
					arl.Roles.Add(new ApiRole { RoleId = r.Id, RoleName = r.Name });
				}
				Response.ContentType = "application/json";
				return JsonConvert.SerializeObject(arl);
			}
			NotFound();
			return null;
		}

		[HttpGet("emptyconfig")]
		public JObject EmptyConfig()
		{
			// allow cross-domain shit for w/e reason
			Response.Headers.Add("Access-Control-Allow-Origin", "*");

			Response.ContentType = "application/json";
			return JObject.FromObject(new GuildSettings() { Prefix = "data served by ModCore."});
		}
	}
}
