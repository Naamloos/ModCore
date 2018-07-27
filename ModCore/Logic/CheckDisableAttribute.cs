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
#if !HSN_DEBUG_ALWAYS_COMMAND_CHECK
			if (ctx.Member.IsOwner || ctx.User.Id == ctx.Client.CurrentApplication.Owner.Id) return true;
#endif
			
			// don't use GetGuildSettings here
			// TODO i wrote this a long time ago, and forgot why not to do that.
			using (var db = Program.ModCore.CreateGlobalContext())
			{
				var stg = db.GuildConfig.SingleOrDefault(xc => (ulong) xc.GuildId == ctx.Guild.Id)?.GetSettings();
				if ((stg?.DisabledCommands?.Count ?? 0) == 0) return true;

				// the parts to a command eg {"config", "linkfilter", "modules", "all", "on"}
				var parts = CommandParts(ctx.Command).ToArray();
				// we take the parts and build a tree, eg
				// config
				// config linkfilter
				// config linkfilter modules
				// config linkfilter modules all
				// config linkfilter modules all on
				var partsStructure = parts.Select((_, i) => string.Join(' ', parts.Take(i+1)));
				// we check each item of the tree and if any command with that id is in the blacklist, we fail
				foreach (var commandPart in partsStructure)
				{
					var cmd = await db.CommandIds.FindAsync(commandPart);
					if (cmd == null || !stg.DisabledCommands.Contains(cmd.Id)) continue;
					
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
