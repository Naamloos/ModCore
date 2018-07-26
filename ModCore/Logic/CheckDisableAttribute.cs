using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModCore.Database;

namespace ModCore.Logic
{
	/// <inheritdoc />
	/// <summary>
	/// Defines that usage of this command is restricted to the owner of the bot.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
	public sealed class CheckDisableAttribute : CheckBaseAttribute
	{
		public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
		{
			if (ctx.Member.IsOwner || ctx.User.Id == ctx.Client.CurrentApplication.Owner.Id) return true;
			
			// don't use GetGuildSettings here
			// TODO i wrote this a long time ago, and forgot why not to do that.
			using (var db = Program.ModCore.CreateGlobalContext())
			{
				var stg = db.GuildConfig.SingleOrDefault(xc => (ulong) xc.GuildId == ctx.Guild.Id)?.GetSettings();
				if ((stg?.DisabledCommands?.Count ?? 0) == 0) return true;
			
				foreach (var commandPart in CommandParts(ctx.Command))
				{
					var cmd = db.CommandIds.Find(commandPart);
					if (cmd == null || !stg.DisabledCommands.Contains(cmd.Id)) continue;
				}
				
				foreach (var disabledCommand in stg.DisabledCommands)
				{
					var cmd = db.CommandIds.Find(disabledCommand);
					if (cmd == null || !ctx.Command.QualifiedName.StartsWith(cmd.Command)) continue;
					
					await ctx.RespondAsync("Sorry! that command has been disabled in this guild.");
					return false;
				}
			}

			return true;
		}

		private static IEnumerable<string> CommandParts(Command c)
		{
			var l = new List<string>();
			while (true)
			{
				l.Add(c.Name.ToLowerInvariant());
				c = c.Parent;
				if (c == null) break;
			}

			l.Reverse();
			return l;
		}
	}
}
