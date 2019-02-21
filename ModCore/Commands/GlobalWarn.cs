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
	// I'm considering removing this. 
	// Nothing personal to JCryer, but I think it's dangerous to hand this much power to random guild owners.
	// TBD
	[Group("globalwarn"), Aliases("gw", "gwarn", "globalw"), Description("Commands to add or remove globalwarns."),
			RequireUserPermissions(Permissions.Administrator), RequireBotPermissions(Permissions.BanMembers), CheckDisable]
	public class GlobalWarn : BaseCommandModule
	{
		private DatabaseContextBuilder Database { get; }
		private SharedData Shared;

		public GlobalWarn(DatabaseContextBuilder db, SharedData sd)
		{
			this.Database = db;
			this.Shared = sd;
		}

		[Command("add"), Description("Adds the specified user to a global watchlist."), CheckDisable]
		public async Task AddAsync(CommandContext ctx, [Description("Member to warn about")]DiscordMember m,
	   [RemainingText, Description("Reason to warn about this member")] string reason = "")
		{
			var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
			if (cfg.GlobalWarn.WarnLevel == GlobalWarnLevel.None || cfg.GlobalWarn.Enable)
				await ctx.SafeRespondAsync("You do not have globalwarn enabled on this server.");

			bool issuedBefore = false;
			using (var db = this.Database.CreateContext())
				issuedBefore = db.Bans.Any(x => x.GuildId == (long)ctx.Guild.Id && x.UserId == (long)m.Id);
			if (issuedBefore)
			{
				await ctx.SafeRespondAsync("You have already warned about this user! Stop picking on them...");
				return;
			}
			if (ctx.Member.Id == m.Id)
			{
				await ctx.SafeRespondAsync("You can't do that to yourself! You have so much to live for!");
				return;
			}

			var ban = new DatabaseBan
			{
				GuildId = (long)ctx.Guild.Id,
				UserId = (long)m.Id,
				IssuedAt = DateTime.Now,
				BanReason = reason
			};
			using (var db = this.Database.CreateContext())
			{
				db.Bans.Add(ban);
				await db.SaveChangesAsync();
			}

			var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
			var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
			await m.ElevatedMessageAsync($"You've been banned from {ctx.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the following reason:\n```\n{reason}\n```")}");
			await ctx.Guild.BanMemberAsync(m, 7, $"{ustr}{rstr}");
			await ctx.SafeRespondAsync($"Banned and issued global warn about user {m.DisplayName} (ID:{m.Id})");

			await ctx.LogActionAsync($"Banned and issued global warn about user {m.DisplayName} (ID:{m.Id})\n{rstr}\n");
			await GlobalWarnUpdateAsync(ctx, m, true);
		}

		[Command("remove"), Description("Removes the specified user from the global watchlist."), CheckDisable]
		public async Task RemoveAsync(CommandContext ctx, [Description("Member to warn about")]DiscordMember m)
		{
			var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
			if (cfg.GlobalWarn.WarnLevel == GlobalWarnLevel.None || cfg.GlobalWarn.Enable)
				await ctx.SafeRespondAsync("You do not have globalwarn enabled on this server.");

			bool issuedBefore = false;
			using (var db = this.Database.CreateContext())
				issuedBefore = db.Bans.Any(x => x.GuildId == (long)ctx.Guild.Id && x.UserId == (long)m.Id);
			if (issuedBefore)
			{
				await ctx.SafeRespondAsync("You have already warned about this user! Stop picking on them...");
				return;
			}
			using (var db = this.Database.CreateContext())
			{
				db.Bans.Remove(db.Bans.First(x => x.GuildId == (long)ctx.Guild.Id && x.UserId == (long)m.Id));
				await db.SaveChangesAsync();
			}

			var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
			await m.ElevatedMessageAsync($"You've been unbanned from {ctx.Guild.Name}.");
			await ctx.Guild.UnbanMemberAsync(m, $"{ustr}");
			await ctx.SafeRespondAsync($"Unbanned and retracted global warn about user {m.DisplayName} (ID:{m.Id})");

			await ctx.LogActionAsync($"Unbanned and retracted global warn about user {m.DisplayName} (ID:{m.Id})\n");
			await GlobalWarnUpdateAsync(ctx, m, false);
		}

		private async Task GlobalWarnUpdateAsync(CommandContext ctx, DiscordMember m, bool banNotify)
		{
			DatabaseBan[] bans;
			using (var db = this.Database.CreateContext())
			{
				bans = db.Bans.Where(x => x.UserId == (long)m.Id).ToArray();

				var prevowns = new List<ulong>();
				int count = 0;
				var guilds = Shared.ModCore.Shards.SelectMany(x => x.Client.Guilds.Values);
				foreach (var b in bans)
				{
					var g = guilds.First(x => x.Id == (ulong)b.GuildId);
					if (prevowns.Contains(g.Owner.Id))
						continue;
					count++;
					prevowns.Add(g.Owner.Id);
				}
				if (banNotify)
				{
					if (count > 2)
					{
						foreach (DiscordGuild g in guilds)
						{
							try
							{
								var settings = g.GetGuildSettings(db) ?? new GuildSettings();
								DiscordMember guildmember = await g.GetMemberAsync(m.Id);

								if (guildmember != null && g.Id != ctx.Guild.Id && settings.GlobalWarn.Enable)
								{
									var embed = new DiscordEmbedBuilder()
										.WithColor(DiscordColor.MidnightBlue)
										.WithTitle($"WARNING: @{m.Username}#{m.Discriminator} - ID: {m.Id}");

									var banString = new StringBuilder();
									foreach (DatabaseBan ban in bans) banString.Append($"[{ban.GuildId} - {ban.BanReason}] ");
									embed.AddField("Bans", banString.ToString());

									if (settings.GlobalWarn.WarnLevel == GlobalWarnLevel.Owner)
									{
										await g.Owner.ElevatedMessageAsync(embed: embed);
									}
									else if (settings.GlobalWarn.WarnLevel == GlobalWarnLevel.JoinLog)
									{
										await g.Channels.First(x => x.Id == (ulong)settings.JoinLog.ChannelId).ElevatedMessageAsync(embed: embed);
									}
								}
							}
							catch
							{
								// TODO: Make SSG Proud
							}
						}
					}
				}
				else
				{
					if (count >= 0)
					{
						foreach (DiscordGuild g in guilds)
						{
							try
							{
								var settings = g.GetGuildSettings(db) ?? new GuildSettings();
								DiscordUser user = await ctx.Client.GetUserAsync(m.Id);

								if (user != null && g.Id != ctx.Guild.Id && settings.GlobalWarn.Enable)
								{
									var embed = new DiscordEmbedBuilder()
										.WithColor(DiscordColor.MidnightBlue)
										.WithTitle($"INFORMATION: @{m.Username}#{m.Discriminator} - ID: {m.Id}")
										.WithDescription($"User has been *unbanned*, with global warn removed, from {ctx.Guild.Name}.");

									if (count == 0)
									{
										embed.Description += "\nHe is now banned on no guilds.";
									}
									else
									{
										var banString = new StringBuilder();
										foreach (DatabaseBan ban in bans) banString.Append($"[{ban.GuildId} - {ban.BanReason}] ");
										embed.AddField("Bans", banString.ToString());
									}
									if (settings.GlobalWarn.WarnLevel == GlobalWarnLevel.Owner)
									{
										await g.Owner.ElevatedMessageAsync(embed: embed);
									}
									else if (settings.GlobalWarn.WarnLevel == GlobalWarnLevel.JoinLog)
									{
										await g.Channels.First(x => x.Id == (ulong)settings.JoinLog.ChannelId).ElevatedMessageAsync(embed: embed);
									}
								}
							}
							catch
							{
								// TODO: Make SSG Proud
							}
						}
					}
				}
			}
		}
	}
}
