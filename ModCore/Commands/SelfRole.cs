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
		public async Task GiveAsync(CommandContext context, [RemainingText, Description("Role you want to give to yourself")] DiscordRole role)
		{
			var config = context.GetGuildSettings() ?? new GuildSettings(); ;
			if (config.SelfRoles.Contains(role.Id))
			{
				if (context.Member.Roles.Any(x => x.Id == role.Id))
				{
					await context.SafeRespondUnformattedAsync("⚠️ You already have that role!");
					return;
				}
				if (context.Guild.CurrentMember.Roles.Any(x => x.Position >= role.Position))
				{
					await context.Member.GrantRoleAsync(role, "AutoRole granted.");
					await context.SafeRespondAsync($"✅ Granted you the role `{role.Name}`.");
				}
				else
					await context.SafeRespondUnformattedAsync("⚠️ Can't grant you this role because that role is above my highest role!");
			}
			else
			{
				await context.SafeRespondUnformattedAsync("⚠️ You can't grant yourself that role!");
			}
		}

		[Command("take"), Aliases("t"), Description("Removes a specified role from the command callee, if " +
																 "ModCore has been configured to allow so."), CheckDisable]
		public async Task TakeAsync(CommandContext context, [RemainingText, Description("Role you want to take from yourself")] DiscordRole role)
		{
			var config = context.GetGuildSettings() ?? new GuildSettings(); ;

			if (config.SelfRoles.Contains(role.Id))
			{
				if (context.Member.Roles.All(x => x.Id != role.Id))
				{
					await context.SafeRespondUnformattedAsync("⚠️ You don't have that role!");
					return;
				}
				if (context.Guild.CurrentMember.Roles.Any(x => x.Position >= role.Position))
				{
					await context.Member.RevokeRoleAsync(role, "AutoRole revoke.");
					await context.SafeRespondAsync($"✅ Revoked your role: `{role.Name}`.");
				}
				else
					await context.SafeRespondUnformattedAsync("⚠️ Can't take this role because that role is above my highest role!");
			}
			else
			{
				await context.SafeRespondUnformattedAsync("⚠️ You can't revoke that role!");
			}
		}

		[Command("list"), Aliases("l"), Description("Lists all available selfroles, if any."), CheckDisable]
		public async Task ListAsync(CommandContext context)
		{
			GuildSettings config;
			config = context.GetGuildSettings() ?? new GuildSettings();
			if (config.SelfRoles.Any())
			{
				var embed = new DiscordEmbedBuilder
				{
					Title = context.Guild.Name,
					Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail() { Url = context.Guild.IconUrl },
					Description = "Available SelfRoles:"
				};

				var roles = config.SelfRoles
					.Select(context.Guild.GetRole)
					.Where(x => x != null)
					.Select(x => x.Mention);

				embed.AddField("Available SelfRoles", string.Join(", ", roles), true);
				await context.ElevatedRespondAsync(embed: embed);
			}
			else
			{
				await context.SafeRespondUnformattedAsync("⚠️ No available selfroles.");
			}
		}
	}
}
