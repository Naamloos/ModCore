using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModCore.Logic.Extensions;

namespace ModCore.Commands
{
	[Group("selfrole"), Description("Commands to give or take selfroles."), RequireBotPermissions(Permissions.ManageRoles), CheckDisable]
	public class SelfRole : BaseCommandModule
	{
		private DatabaseContextBuilder Database { get; }

		public SelfRole(DatabaseContextBuilder db)
		{
			this.Database = db;
		}

		[Command("give"), Aliases("g"), Description("Gives the command callee a specified role, if " +
																 "ModCore has been configured to allow so."), CheckDisable]
		public async Task GiveAsync(CommandContext ctx, [RemainingText, Description("Role you want to give to yourself")] DiscordRole role)
		{
			var cfg = ctx.GetGuildSettings() ?? new GuildSettings(); ;
			if (cfg.SelfRoles.Contains(role.Id))
			{
				if (ctx.Member.Roles.Any(x => x.Id == role.Id))
				{
					await ctx.SafeRespondUnformattedAsync("You already have that role!");
					return;
				}
				if (ctx.Guild.CurrentMember.Roles.Any(x => x.Position >= role.Position))
				{
					await ctx.Member.GrantRoleAsync(role, "AutoRole granted.");
					await ctx.SafeRespondAsync($"Granted you the role `{role.Name}`.");
				}
				else
					await ctx.SafeRespondUnformattedAsync("Can't grant you this role because that role is above my highest role!");
			}
			else
			{
				await ctx.SafeRespondUnformattedAsync("You can't grant yourself that role!");
			}
		}

		[Command("take"), Aliases("t"), Description("Removes a specified role from the command callee, if " +
																 "ModCore has been configured to allow so."), CheckDisable]
		public async Task TakeAsync(CommandContext ctx, [RemainingText, Description("Role you want to take from yourself")] DiscordRole role)
		{
			var cfg = ctx.GetGuildSettings() ?? new GuildSettings(); ;

			if (cfg.SelfRoles.Contains(role.Id))
			{
				if (ctx.Member.Roles.All(x => x.Id != role.Id))
				{
					await ctx.SafeRespondUnformattedAsync("You don't have that role!");
					return;
				}
				if (ctx.Guild.CurrentMember.Roles.Any(x => x.Position >= role.Position))
				{
					await ctx.Member.RevokeRoleAsync(role, "AutoRole revoke.");
					await ctx.SafeRespondAsync($"Revoked your role: `{role.Name}`.");
				}
				else
					await ctx.SafeRespondUnformattedAsync("Can't take this role because that role is above my highest role!");
			}
			else
			{
				await ctx.SafeRespondUnformattedAsync("You can't revoke that role!");
			}
		}

		[Command("list"), Aliases("l"), Description("Lists all available selfroles, if any."), CheckDisable]
		public async Task ListAsync(CommandContext ctx)
		{
			GuildSettings cfg;
			cfg = ctx.GetGuildSettings() ?? new GuildSettings();
			if (cfg.SelfRoles.Any())
			{
				var embed = new DiscordEmbedBuilder
				{
					Title = ctx.Guild.Name,
					ThumbnailUrl = ctx.Guild.IconUrl,
					Description = "Available SelfRoles:"
				};
				var roles = cfg.SelfRoles
					.Select(ctx.Guild.GetRole)
					.Where(x => x != null)
					.Select(x => x.Mention);

				embed.AddField("Available SelfRoles", string.Join(", ", roles), true);
				await ctx.ElevatedRespondAsync(embed: embed);
			}
			else
			{
				await ctx.SafeRespondUnformattedAsync("No available selfroles.");
			}
		}
	}
}
