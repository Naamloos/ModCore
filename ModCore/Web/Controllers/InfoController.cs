using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.AspNetCore.Mvc;
using ModCore.CoreApi.Entities;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Web.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModCore.Web.Controllers
{
	[Route("info")]
	public class InfoController : Controller
    {
		private ModCore ModCore;

		public InfoController(ModCore modcore)
		{
			ModCore = modcore;
		}

		public string Index()
		{
			return "Information generated from ModCore's internals.";
		}

		[HttpGet("invite")]
		public string Invite()
		{
			Response.Redirect($"https://discordapp.com/oauth2/authorize?client_id=359828546719449109&scope=bot&permissions=8");
			return "Redirecting to invite...";
		}

		[HttpGet("permissions")]
		public ActionResult Perms()
		{
			List<ApiCommand> apiCmd = new List<ApiCommand>();
			// TODO optimize (cache the info)
			var cmd = this.ModCore.SharedData.Commands.Select(x => x.cmd);

			foreach(var c in cmd)
			{
				if (!c.ExecutionChecks.Any(x => x.GetType() == typeof(RequireOwnerAttribute)))
					apiCmd.Add(new ApiCommand()
					{
						FullName = c.QualifiedName,
						UserPerms = GetAllCmdUsrPerms(c).ToPermissionString(),
						BotPerms = GetAllCmdBotPerms(c).ToPermissionString(),
						BothPerms = GetAllCmdPerms(c).ToPermissionString()
					});
			}

			return Content(JsonConvert.SerializeObject(apiCmd), "application/json");
		}

		[HttpGet("guilds")]
		public string Guilds()
		{
			return $"ModCore is currently serving {ModCore.Shards.Select(x => x.Client.Guilds.Count).Sum()} guilds over {ModCore.Shards.Count} shards!";
		}

		private Permissions GetAllCmdPerms(Command cmd)
		{
			Permissions perm = Permissions.None;
			if (cmd.Parent != null)
				perm = perm | GetAllCmdPerms(cmd.Parent);
			if(cmd.ExecutionChecks.Any(x => x.GetType() == typeof(RequirePermissionsAttribute)))
			{
				var permchek = (RequirePermissionsAttribute)cmd.ExecutionChecks.First(x => x.GetType() == typeof(RequirePermissionsAttribute));
				perm = perm | permchek.Permissions;
			}

			return perm;
		}

		private Permissions GetAllCmdBotPerms(Command cmd)
		{
			Permissions perm = Permissions.None;
			if (cmd.Parent != null)
				perm = perm | GetAllCmdBotPerms(cmd.Parent);
			if (cmd.ExecutionChecks.Any(x => x.GetType() == typeof(RequireBotPermissionsAttribute)))
			{
				var permchek = (RequireBotPermissionsAttribute)cmd.ExecutionChecks.First(x => x.GetType() == typeof(RequireBotPermissionsAttribute));
				perm = perm | permchek.Permissions;
			}

			return perm;
		}

		private Permissions GetAllCmdUsrPerms(Command cmd)
		{
			Permissions perm = Permissions.None;
			if (cmd.Parent != null)
				perm = perm | GetAllCmdUsrPerms(cmd.Parent);
			if (cmd.ExecutionChecks.Any(x => x.GetType() == typeof(RequireUserPermissionsAttribute)))
			{
				var permchek = (RequireUserPermissionsAttribute)cmd.ExecutionChecks.First(x => x.GetType() == typeof(RequireUserPermissionsAttribute));
				perm = perm | permchek.Permissions;
			}

			return perm;
		}
	}
}
