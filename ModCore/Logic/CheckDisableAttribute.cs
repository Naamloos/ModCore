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
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class CheckDisableAttribute : CheckBaseAttribute
	{
		public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
		{
			// don't use GetGuildSettings here
			using (var db = Program.ModCore.CreateGlobalContext())
			{
				var stg = db.GuildConfig.SingleOrDefault(xc => (ulong) xc.GuildId == ctx.Guild.Id)?.GetSettings();
				if (stg?.DisabledCommands == null || stg?.DisabledCommands?.Count == 0) return true;
			
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
	}
}
