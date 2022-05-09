using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
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

namespace ModCore.Web.Controllers
{
	[Route("info")]
	public class InfoController : Controller
    {
		private ModCore core;

		public InfoController(CoreContainer cont)
		{
			this.core = cont.mcore;
		}

		public string Index()
		{
			return "Information generated from ModCore's internals.";
		}

		[HttpGet("invite")]
		public string Invite()
		{
			var perms = Permissions.None;
			foreach (var p in this.core.SharedData.AllPermissions)
			{
				Console.WriteLine(p);
				perms = perms | p;
			}
			var permsvalue = (long)perms;

			Response.Redirect($"https://discordapp.com/oauth2/authorize?client_id=359828546719449109&scope=bot&permissions={permsvalue}");
			return "Redirecting to generated invite...";
		}

		[HttpGet("permissions")]
		public ActionResult Perms()
		{
			var cmd = this.core.SharedData.Commands.Select(x => x.cmd);
			StringBuilder data = new StringBuilder();
			data.AppendLine("<h1>Commands and their required permissions</h1>");
			data.AppendLine("<ul>");

			foreach(var c in cmd)
			{
				if(!c.ExecutionChecks.Any(x => x.GetType() == typeof(RequireOwnerAttribute)))
				data.AppendLine($"<li><h4>{c.QualifiedName}</h4> Bot+User: {GetAllCmdPerms(c)}" +
					$" Bot: {GetAllCmdBotPerms(c)} User: {GetAllCmdUsrPerms(c)}</li>");
			}

			data.AppendLine("</ul>");

			return Content(data.ToString(), "text/html");
		}

		[HttpGet("guilds")]
		public string Guilds()
		{
			return $"ModCore is currently serving {core.Shards.Select(x => x.Client.Guilds.Count).Sum()} guilds over {core.Shards.Count} shards!";
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
