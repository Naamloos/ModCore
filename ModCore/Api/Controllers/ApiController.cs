using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using ModCore.Api.Entities;

namespace ModCore.Api.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private ModCore ModCore;

        public ApiController(ModCore modcore)
        {
            ModCore = modcore;
        }

        public ActionResult Index()
        {
            return Content("ModCore API", "application/json");
        }

        // GET ping
        [HttpGet("ping")]
        public ActionResult Ping()
        {
            return Content("pong!", "application/json");
        }

        [HttpGet("prefix")]
        public ActionResult Dp()
        {
			var prefix = new ApiPrefix() { Prefix = ModCore.SharedData.DefaultPrefix };

			return Content(JsonConvert.SerializeObject(prefix), "application/json");
        }

		[HttpGet("invite")]
		public ActionResult Invite()
		{
			var invite = new ApiInvite() { Invite = "https://discordapp.com/oauth2/authorize?client_id=359828546719449109&scope=bot&permissions=8" };
			return Content(JsonConvert.SerializeObject(invite), "application/json");
		}

		[HttpGet("invite/redirect")]
		public string InviteRedirect()
		{
			Response.Redirect($"https://discordapp.com/oauth2/authorize?client_id=359828546719449109&scope=bot&permissions=8");
			return "Redirecting to invite...";
		}

		[HttpGet("permissions")]
		public ActionResult Perms()
		{
			List<ApiCommandData> apiCmd = new List<ApiCommandData>();
			// TODO optimize (cache the info)
			var cmd = this.ModCore.Shards[0].Commands.RegisteredCommands.Values;

			foreach (var c in cmd)
			{
				if (!c.ExecutionChecks.Any(x => x.GetType() == typeof(RequireOwnerAttribute)))
					apiCmd.Add(new ApiCommandData()
					{
						FullName = c.QualifiedName,
						UserPerms = getUserPermissions(c).ToPermissionString(),
						BotPerms = getBotPermissions(c).ToPermissionString(),
						BothPerms = getCombinedPermissions(c).ToPermissionString()
					});
			}

			return Content(JsonConvert.SerializeObject(apiCmd), "application/json");
		}

		[HttpGet("metadata")]
		public ActionResult Guilds()
		{
			var metadata = new ApiMetadata()
			{
				Shards = ModCore.Shards.Count,
				Guilds = ModCore.Shards.Sum(x => x.Client.Guilds.Count)
			};

			return Content(JsonConvert.SerializeObject(metadata), "application/json");
		}

		private Permissions getCombinedPermissions(Command cmd)
		{
			Permissions perm = Permissions.None;
			if (cmd.Parent != null)
				perm = perm | getCombinedPermissions(cmd.Parent);
			if (cmd.ExecutionChecks.Any(x => x.GetType() == typeof(RequirePermissionsAttribute)))
			{
				var permchek = (RequirePermissionsAttribute)cmd.ExecutionChecks.First(x => x.GetType() == typeof(RequirePermissionsAttribute));
				perm = perm | permchek.Permissions;
			}

			return perm;
		}

		private Permissions getBotPermissions(Command cmd)
		{
			Permissions perm = Permissions.None;
			if (cmd.Parent != null)
				perm = perm | getBotPermissions(cmd.Parent);
			if (cmd.ExecutionChecks.Any(x => x.GetType() == typeof(RequireBotPermissionsAttribute)))
			{
				var permchek = (RequireBotPermissionsAttribute)cmd.ExecutionChecks.First(x => x.GetType() == typeof(RequireBotPermissionsAttribute));
				perm = perm | permchek.Permissions;
			}

			return perm;
		}

		private Permissions getUserPermissions(Command cmd)
		{
			Permissions perm = Permissions.None;
			if (cmd.Parent != null)
				perm = perm | getUserPermissions(cmd.Parent);
			if (cmd.ExecutionChecks.Any(x => x.GetType() == typeof(RequireUserPermissionsAttribute)))
			{
				var permchek = (RequireUserPermissionsAttribute)cmd.ExecutionChecks.First(x => x.GetType() == typeof(RequireUserPermissionsAttribute));
				perm = perm | permchek.Permissions;
			}

			return perm;
		}
	}
}
