using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Logic
{
	/// <summary>
	/// Defines that usage of this command is restricted to the owner of the bot.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class CheckDisableAttribute : CheckBaseAttribute
	{
		public async override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
		{
			var stg = ctx.GetGuildSettings();
			if (stg?.DisabledCommands != null && stg.DisabledCommands.Any(x => ctx.Command.QualifiedName.StartsWith(x.ToLower())))
			{
				await ctx.RespondAsync("Sorry! that command has been disabled in this guild.");
				return false;
			}
			return true;
		}
	}
}
