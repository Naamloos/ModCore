using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Humanizer;
using Humanizer.Localisation;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Listeners;
using ModCore.Logic;
using ModCore.Logic.Utils;

namespace ModCore.Commands
{
	public class Main : BaseCommandModule
	{
		private static readonly Regex SpaceReplacer = new Regex(" {2,}", RegexOptions.Compiled);

		public SharedData Shared { get; }
		public DatabaseContextBuilder Database { get; }
		public InteractivityExtension Interactivity { get; }
		public StartTimes StartTimes { get; }

		public Main(SharedData shared, DatabaseContextBuilder db, InteractivityExtension interactive,
			StartTimes starttimes)
		{
			this.Database = db;
			this.Shared = shared;
			this.Interactivity = interactive;
			this.StartTimes = starttimes;
		}

		[Command("ping"), Description("Check ModCore's API connection status.")]
		public async Task PingAsync(CommandContext ctx)
		{
			await ctx.SafeRespondAsync($"Pong: ({ctx.Client.Ping}) ms.");
		}

		[Command("uptime"), Description("Check ModCore's uptime."), Aliases("u")]
		public async Task UptimeAsync(CommandContext ctx)
		{
			var st = this.StartTimes;
			var bup = DateTimeOffset.Now.Subtract(st.ProcessStartTime);
			var sup = DateTimeOffset.Now.Subtract(st.SocketStartTime);

			// Needs improvement
			await ctx.SafeRespondAsync(
				$"Program uptime: {string.Format("{0} days, {1}", bup.ToString("dd"), bup.ToString(@"hh\:mm\:ss"))}\n" +
				$"Socket uptime: {string.Format("{0} days, {1}", sup.ToString("dd"), sup.ToString(@"hh\:mm\:ss"))}");
		}

		[Command("invite"), Description("Get an invite to this ModCore instance. Sharing is caring!"), Aliases("inv")]
		public async Task InviteAsync(CommandContext ctx)
		{
			//TODO replace with a link to a nice invite builder!
			// what the hell is an invite builder? - chris
			var app = ctx.Client.CurrentApplication;
			if (app.IsPublic != null && (bool)app.IsPublic)
				await ctx.SafeRespondAsync(
					$"Add ModCore to your server!\n<https://discordapp.com/oauth2/authorize?client_id={app.Id}&scope=bot>");
			else
				await ctx.SafeRespondAsync("I'm sorry Mario, but this instance of ModCore has been set to private!");
		}

		[Group("purge"), Aliases("p"), RequirePermissions(Permissions.ManageMessages)]
		class Purge : BaseCommandModule
		{
			[GroupCommand, Description("Delete an amount of messages from the current channel.")]
			public async Task ExecuteGroupAsync(CommandContext ctx, [Description("Amount of messages to remove (max 100)")]int limit = 50,
				[Description("Amount of messages to skip")]int skip = 0)
			{
				var i = 0;
				var ms = await ctx.Channel.GetMessagesBeforeAsync(ctx.Message.Id, limit);
				var deletThis = new List<DiscordMessage>();
				foreach (var m in ms)
				{
					if (i < skip)
						i++;
					else
						deletThis.Add(m);
				}
				if (deletThis.Any())
					await ctx.Channel.DeleteMessagesAsync(deletThis, "Purged messages.");
				var resp = await ctx.SafeRespondAsync("Latest messages deleted.");
				await Task.Delay(2000);
				await resp.DeleteAsync("Purge command executed.");
				await ctx.Message.DeleteAsync("Purge command executed.");

				await ctx.LogActionAsync($"Purged messages.\nChannel: #{ctx.Channel.Name} ({ctx.Channel.Id})");
			}

			[Command("user"), Description("Delete an amount of messages by an user."), Aliases("u", "pu")]
			public async Task PurgeUserAsync(CommandContext ctx, [Description("User to delete messages from")]DiscordUser user,
			[Description("Amount of messages to remove (max 100)")]int limit = 50, [Description("Amount of messages to skip")]int skip = 0)
			{
				var i = 0;
				var ms = await ctx.Channel.GetMessagesBeforeAsync(ctx.Message.Id, limit);
				var deletThis = new List<DiscordMessage>();
				foreach (var m in ms)
				{
					if (user != null && m.Author.Id != user.Id) continue;
					if (i < skip)
						i++;
					else
						deletThis.Add(m);
				}
				if (deletThis.Any())
					await ctx.Channel.DeleteMessagesAsync(deletThis,
						$"Purged messages by {user?.Username}#{user?.Discriminator} (ID:{user?.Id})");
				var resp = await ctx.SafeRespondAsync($"Latest messages by {user?.Mention} (ID:{user?.Id}) deleted.");
				await Task.Delay(2000);
				await resp.DeleteAsync("Purge command executed.");
				await ctx.Message.DeleteAsync("Purge command executed.");

				await ctx.LogActionAsync(
					$"Purged messages.\nUser: {user?.Username}#{user?.Discriminator} (ID:{user?.Id})\nChannel: #{ctx.Channel.Name} ({ctx.Channel.Id})");
			}

			[Command("regexp"), Description(
			 "For power users! Delete messages from the current channel by regular expression match. " +
			 "Pass a Regexp in ECMAScript ( /expression/flags ) format, or simply a regex string " +
			 "in quotes."), Aliases("purgeregex", "pr", "r")]
			public async Task PurgeRegexpAsync(CommandContext ctx, [Description("Your regex")] string regexp,
			[Description("Amount of messages to remove (max 100)")]int limit = 50, [Description("Amount of messages to skip")]int skip = 0)
			{
				// TODO add a flag to disable CultureInvariant.
				var regexOptions = RegexOptions.CultureInvariant;
				// kept here for displaying in the result
				var flags = "";

				if (string.IsNullOrEmpty(regexp))
				{
					await ctx.SafeRespondAsync("RegExp is empty");
					return;
				}
				var blockType = regexp[0];
				if (blockType == '"' || blockType == '/')
				{
					// token structure
					// "regexp" limit? skip?
					// /regexp/ limit? skip?
					// /regexp/ flags limit? skip? 
					var tokens = Tokenize(SpaceReplacer.Replace(regexp, " ").Trim(), ' ', blockType);
					regexp = tokens[0];
					if (tokens.Count > 1)
					{
						// parse flags only in ECMAScript regexp literal
						if (blockType == '/')
						{
							// if tokens[1] is a valid integer then it's `limit`. otherwise it's `flags`, and we remove it
							// for the other bits.
							flags = tokens[1];
							if (!int.TryParse(flags, out var _))
							{
								// remove the flags element
								tokens.RemoveAt(1);

								if (flags.Contains('m'))
								{
									regexOptions |= RegexOptions.Multiline;
								}
								if (flags.Contains('i'))
								{
									regexOptions |= RegexOptions.IgnoreCase;
								}
								if (flags.Contains('s'))
								{
									regexOptions |= RegexOptions.Singleline;
								}
								if (flags.Contains('x'))
								{
									regexOptions |= RegexOptions.ExplicitCapture;
								}
								if (flags.Contains('r'))
								{
									regexOptions |= RegexOptions.RightToLeft;
								}
								// for debugging only
								if (flags.Contains('c'))
								{
									regexOptions |= RegexOptions.Compiled;
								}
							}
						}

						if (int.TryParse(tokens[1], out var result))
						{
							limit = result;
						}
						else
						{
							await ctx.SafeRespondAsync(tokens[1] + " is not a valid int");
							return;
						}
						if (tokens.Count > 2)
						{
							if (int.TryParse(tokens[2], out var res2))
							{
								skip = res2;
							}
							else
							{
								await ctx.SafeRespondAsync(tokens[2] + " is not a valid int");
								return;
							}
						}
					}
				}
				var regexCompiled = new Regex(regexp, regexOptions);

				var i = 0;
				var ms = await ctx.Channel.GetMessagesBeforeAsync(ctx.Message.Id, limit);
				var deletThis = new List<DiscordMessage>();
				foreach (var m in ms)
				{
					if (!regexCompiled.IsMatch(m.Content)) continue;

					if (i < skip)
						i++;
					else
						deletThis.Add(m);
				}
				var resultString =
					$"Purged {deletThis.Count} messages by /{regexp.Replace("/", @"\/").Replace(@"\", @"\\")}/{flags}";
				if (deletThis.Any())
					await ctx.Channel.DeleteMessagesAsync(deletThis, resultString);
				var resp = await ctx.SafeRespondAsync(resultString);
				await Task.Delay(2000);
				await resp.DeleteAsync("Purge command executed.");
				await ctx.Message.DeleteAsync("Purge command executed.");

				await ctx.LogActionAsync(
					$"Purged {deletThis.Count} messages.\nRegex: ```\n{regexp}```\nFlags: {flags}\nChannel: #{ctx.Channel.Name} ({ctx.Channel.Id})");
			}

			[Command("commands"), Description("Purge ModCore's messages."), Aliases("c", "self", "own"),
		 RequirePermissions(Permissions.ManageMessages)]
			public async Task CleanAsync(CommandContext ctx)
			{
				var gs = ctx.GetGuildSettings() ?? new GuildSettings();
				var prefix = gs?.Prefix ?? "?>";
				var ms = await ctx.Channel.GetMessagesBeforeAsync(ctx.Message.Id, 100);
				var deletThis = ms.Where(m => m.Author.Id == ctx.Client.CurrentUser.Id || m.Content.StartsWith(prefix))
					.ToList();
				if (deletThis.Any())
					await ctx.Channel.DeleteMessagesAsync(deletThis, "Cleaned up commands");
				var resp = await ctx.SafeRespondAsync("Latest messages deleted.");
				await Task.Delay(2000);
				await resp.DeleteAsync("Clean command executed.");
				await ctx.Message.DeleteAsync("Clean command executed.");

				await ctx.LogActionAsync();
			}

			[Command("bots"), Description("Purge messages from all bots in this channel"), Aliases("b", "bot"),
		 RequirePermissions(Permissions.ManageMessages)]
			public async Task PurgeBotsAsync(CommandContext ctx)
			{
				var gs = ctx.GetGuildSettings() ?? new GuildSettings();
				var prefix = gs?.Prefix ?? "?>";
				var ms = await ctx.Channel.GetMessagesBeforeAsync(ctx.Message.Id, 100);
				var deletThis = ms.Where(m => m.Author.IsBot || m.Content.StartsWith(prefix))
					.ToList();
				if (deletThis.Any())
					await ctx.Channel.DeleteMessagesAsync(deletThis, "Cleaned up commands");
				var resp = await ctx.SafeRespondAsync("Latest messages deleted.");
				await Task.Delay(2000);
				await resp.DeleteAsync("Purge bot command executed.");
				await ctx.Message.DeleteAsync("Purge bot command executed.");

				await ctx.LogActionAsync();
			}

			[Command("images"), Description("Purge messages with images or attachments on them."), Aliases("i", "imgs", "img"),
		 RequirePermissions(Permissions.ManageMessages)]
			public async Task PurgeImagesAsync(CommandContext ctx)
			{
				var ms = await ctx.Channel.GetMessagesBeforeAsync(ctx.Message.Id, 100);
				Regex ImageRegex = new Regex(@"\.(png|gif|jpg|jpeg|tiff|webp)");
				var deleteThis = ms.Where(m => ImageRegex.IsMatch(m.Content) || m.Attachments.Any()).ToList();
				if (deleteThis.Any())
					await ctx.Channel.DeleteMessagesAsync(deleteThis, "Purged images");
				var resp = await ctx.SafeRespondAsync("Latest messages deleted.");
				await Task.Delay(2000);
				await resp.DeleteAsync("Image purge command executed.");
				await ctx.Message.DeleteAsync("Image purge command executed.");

				await ctx.LogActionAsync();
			}
		}


		private static List<string> Tokenize(string value, char sep, char block)
		{
			var result = new List<string>();
			var sb = new StringBuilder();
			var insideBlock = false;
			foreach (var c in value)
			{
				if (insideBlock && c == '\\')
				{
					continue;
				}
				if (c == block)
				{
					insideBlock = !insideBlock;
				}
				else if (c == sep && !insideBlock)
				{
					if (sb.IsNullOrWhitespace()) continue;
					result.Add(sb.ToString().Trim());
					sb.Clear();
				}
				else
				{
					sb.Append(c);
				}
			}
			if (sb.ToString().Trim().Length > 0)
			{
				result.Add(sb.ToString().Trim());
			}

			return result;
		}



		[Command("ban"), Description("Bans a member."), Aliases("b"), RequirePermissions(Permissions.BanMembers)]
		public async Task BanAsync(CommandContext ctx, [Description("Member to ban")] DiscordMember m,
			[RemainingText, Description("Reason to ban this member")] string reason = "")
		{
			if (ctx.Member.Id == m.Id)
			{
				await ctx.SafeRespondAsync("You can't do that to yourself! You have so much to live for!");
				return;
			}

			var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
			var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
			await m.ElevatedMessageAsync($"You've been banned from {ctx.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the follwing reason:\n```\n{reason}\n```")}");
			await ctx.Guild.BanMemberAsync(m, 7, $"{ustr}{rstr}");
			await ctx.SafeRespondAsync($"Banned user {m.DisplayName} (ID:{m.Id})");

			await ctx.LogActionAsync($"Banned user {m.DisplayName} (ID:{m.Id})\n{rstr}");
		}

		[Command("hackban"), Description("Ban an user by their ID. The user does not need to be in the guild."),
		 Aliases("hb"), RequirePermissions(Permissions.BanMembers)]
		public async Task HackBanAsync(CommandContext ctx, [Description("ID of user to ban")]ulong id,
			[RemainingText, Description("Reason to ban this member")] string reason = "")
		{
			if (ctx.Member.Id == id)
			{
				await ctx.SafeRespondAsync("You can't do that to yourself! You have so much to live for!");
				return;
			}

			var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
			var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
			await ctx.Guild.BanMemberAsync(id, 7, $"{ustr}{rstr}");
			await ctx.SafeRespondAsync("User hackbanned successfully.");

			await ctx.LogActionAsync($"Hackbanned ID: {id}\n{rstr}");
		}

		[Command("kick"), Description("Kicks a member from the guild. Can optionally provide a reason for kick."),
		 Aliases("k"), RequirePermissions(Permissions.KickMembers)]
		public async Task KickAsync(CommandContext ctx, [Description("Member to kick")]DiscordMember m,
			[RemainingText, Description("Reason to kick this member")] string reason = "")
		{
			if (ctx.Member.Id == m.Id)
			{
				await ctx.SafeRespondAsync("You can't do that to yourself! You have so much to live for!");
				return;
			}

			var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
			var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
			await m.ElevatedMessageAsync($"You've been kicked from {ctx.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the follwing reason:\n```\n{reason}\n```")}");
			await m.RemoveAsync($"{ustr}{rstr}");
			await ctx.SafeRespondAsync($"Kicked user {m.DisplayName} (ID:{m.Id})");

			await ctx.LogActionAsync($"Kicked user {m.DisplayName} (ID:{m.Id})\n{rstr}");
		}

		[Command("softban"),
		 Description("Bans then unbans an user from the guild. " +
					 "This will delete their recent messages, but they can join back."), Aliases("sb"),
		 RequireUserPermissions(Permissions.KickMembers), RequireBotPermissions(Permissions.BanMembers)]
		public async Task SoftbanAsync(CommandContext ctx, [Description("Member to softban")]DiscordMember m,
			[RemainingText, Description("Reason to softban this member")] string reason = "")
		{
			if (ctx.Member.Id == m.Id)
			{
				await ctx.SafeRespondAsync("You can't do that to yourself! You have so much to live for!");
				return;
			}

			var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
			var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
			await m.ElevatedMessageAsync($"You've been kicked from {ctx.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the follwing reason:\n```\n{reason}\n```")}");
			await m.BanAsync(7, $"{ustr}{rstr} (softban)");
			await m.UnbanAsync(ctx.Guild, $"{ustr}{rstr}");
			await ctx.SafeRespondAsync($"Softbanned user {m.DisplayName} (ID:{m.Id})");

			await ctx.LogActionAsync($"Softbanned user {m.DisplayName} (ID:{m.Id})\n{rstr}");
		}



		[Group("globalwarn"), Aliases("gw", "gwarn", "globalw"), Description("Commands to add or remove globalwarns."),
			RequireUserPermissions(Permissions.Administrator), RequireBotPermissions(Permissions.BanMembers)]
		public class GlobalWarn : BaseCommandModule
		{
			private DatabaseContextBuilder Database { get; }

			public GlobalWarn(DatabaseContextBuilder db)
			{
				this.Database = db;
			}

			[Command("add"), Description("Adds the specified user to a global watchlist.")]
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

			[Command("remove"), Description("Removes the specified user from the global watchlist.")]
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
					var guilds = ModCore.Shards.SelectMany(x => x.Client.Guilds.Values);
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

		[Command("mute"), Description("Mutes an user indefinitely. This will prevent them from speaking in chat. " +
									  "You might need to set up a mute role, but most of the time ModCore can do it " +
									  "for you."), Aliases("m"), RequirePermissions(Permissions.MuteMembers),
		 RequireBotPermissions(Permissions.ManageRoles)]
		public async Task MuteAsync(CommandContext ctx, [Description("Member to mute")]DiscordMember m,
			[RemainingText, Description("Reason to mute this member")] string reason = "")
		{
			if (ctx.Member.Id == m.Id)
			{
				await ctx.SafeRespondAsync("You can't do that to yourself! You have so much to live for!");
				return;
			}

			var guildSettings = ctx.GetGuildSettings() ?? new GuildSettings();
			if (guildSettings == null)
			{
				await ctx.SafeRespondAsync("Guild is not configured, please configure and rerun");
				return;
			}

			var b = guildSettings.MuteRoleId;
			var mute = ctx.Guild.GetRole(b);
			if (b == 0 || mute == null)
			{
				var setupStatus = await Utils.SetupMuteRole(ctx.Guild, ctx.Member, m);
				mute = setupStatus.Role;
				guildSettings.MuteRoleId = setupStatus.Role.Id;
				await ctx.SafeRespondAsync("Mute role is not configured or missing, " + setupStatus.Message);
				await ctx.SetGuildSettingsAsync(guildSettings);
			}
			await Utils.GuaranteeMuteRoleDeniedEverywhere(ctx.Guild, mute);

			var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
			var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
			await m.ElevatedMessageAsync($"You've been muted in {ctx.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the follwing reason:\n```\n{reason}\n```")}");
			await m.GrantRoleAsync(mute, $"{ustr}{rstr} (mute)");
			await ctx.SafeRespondAsync(
				$"Muted user {m.DisplayName} (ID:{m.Id}) {(reason != "" ? "With reason: " + reason : "")}");

			await ctx.LogActionAsync(
				$"Muted user {m.DisplayName} (ID:{m.Id}) {(reason != "" ? "With reason: " + reason : "")}");
		}

		[Command("unmute"), Description("Unmutes an user previously muted with the mute command. Let them speak!"),
		 Aliases("um"), RequirePermissions(Permissions.MuteMembers),
		 RequireBotPermissions(Permissions.ManageRoles)]
		public async Task UnmuteAsync(CommandContext ctx, [Description("Member to unmute")]DiscordMember m,
			[RemainingText, Description("Reason to unmute this member")] string reason = "")
		{
			if (ctx.Member.Id == m.Id)
			{
				await ctx.SafeRespondAsync("You can't do that to yourself! You have so much to live for!");
				return;
			}

			var gcfg = ctx.GetGuildSettings() ?? new GuildSettings();
			if (gcfg == null)
			{
				await ctx.SafeRespondAsync(
					"Guild is not configured. Adjust this guild's configuration and re-run this command.");
				return;
			}

			var b = gcfg.MuteRoleId;
			var mute = ctx.Guild.GetRole(b);
			if (b == 0 || mute == null)
			{
				await ctx.SafeRespondAsync(
					"Mute role is not configured or missing. Set a correct role and re-run this command.");
				return;
			}

			var t = Timers.FindNearestTimer(TimerActionType.Unmute, m.Id, 0, ctx.Guild.Id, this.Database);
			if (t != null)
				await Timers.UnscheduleTimerAsync(t, ctx.Client, this.Database, this.Shared);

			var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
			var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
			await m.ElevatedMessageAsync($"You've been unmuted in {ctx.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the follwing reason:\n```\n{reason}\n```")}");
			await m.RevokeRoleAsync(mute, $"{ustr}{rstr} (unmute)");
			await ctx.SafeRespondAsync(
				$"Unmuted user {m.DisplayName} (ID:{m.Id}) {(reason != "" ? "With reason: " + reason : "")}");

			await ctx.LogActionAsync(
				$"Unmuted user {m.DisplayName} (ID:{m.Id}) {(reason != "" ? "With reason: " + reason : "")}");
		}

		[Command("leave"), Description("Makes this bot leave the current server. Goodbye moonmen."),
		 RequireUserPermissions(Permissions.Administrator)]
		public async Task LeaveAsync(CommandContext ctx)
		{
			var interactivity = this.Interactivity;
			await ctx.SafeRespondAsync("Are you sure you want to remove modcore from your guild?");
			var m = await interactivity.WaitForMessageAsync(
				x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Member.Id, TimeSpan.FromSeconds(30));

			if (m == null)
				await ctx.SafeRespondAsync("Timed out.");
			else if (m.Message.Content == "yes")
			{
				await ctx.SafeRespondAsync("Thanks for using ModCore. Leaving this guild.");
				await ctx.LogActionAsync("Left your server. Thanks for using ModCore.");
				await ctx.Guild.LeaveAsync();
			}
			else
				await ctx.SafeRespondAsync("Operation canceled by user.");
		}

		[Command("tempban"), Aliases("tb"), Description(
			 "Temporarily bans a member. They will be automatically unbanned " +
			 "after a set amount of time."),
		 RequirePermissions(Permissions.BanMembers)]
		public async Task TempBanAsync(CommandContext ctx, [Description("Member to ban temporarily")]DiscordMember m,
			[Description("How long this member will be banned")]TimeSpan ts, [Description("Why this member got banned")]string reason = "")
		{
			if (ctx.Member.Id == m.Id)
			{
				await ctx.SafeRespondAsync("You can't do that to yourself! You have so much to live for!");
				return;
			}

			var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
			var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
			await m.ElevatedMessageAsync($"You've been temporarily banned from {ctx.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the following reason:\n```\n{reason}\n```")}" +
				$"\nYou can rejoin after {ts.Humanize(4, minUnit: TimeUnit.Second)}");
			await m.BanAsync(7, $"{ustr}{rstr}");
			// Add timer
			var now = DateTimeOffset.UtcNow;
			var dispatchAt = now + ts;

			var reminder = new DatabaseTimer
			{
				GuildId = (long)ctx.Guild.Id,
				ChannelId = 0,
				UserId = (long)m.Id,
				DispatchAt = dispatchAt.LocalDateTime,
				ActionType = TimerActionType.Unban
			};
			reminder.SetData(new TimerUnbanData
			{
				Discriminator = m.Discriminator,
				DisplayName = m.Username,
				UserId = (long)m.Id
			});
			using (var db = this.Database.CreateContext())
			{
				db.Timers.Add(reminder);
				await db.SaveChangesAsync();
			}

			Timers.RescheduleTimers(ctx.Client, this.Database, this.Shared);

			// End of Timer adding
			await ctx.SafeRespondAsync(
				$"Tempbanned user {m.DisplayName} (ID:{m.Id}) to be unbanned in {ts.Humanize(4, minUnit: TimeUnit.Second)}");

			await ctx.LogActionAsync(
				$"Tempbanned user {m.DisplayName} (ID:{m.Id}) to be unbanned in {ts.Humanize(4, minUnit: TimeUnit.Second)}");
		}

		[Command("tempmute"), Aliases("tm"), Description("Temporarily mutes a member. They will be automatically " +
														 "unmuted after a set amount of time. This will prevent them " +
														 "from speaking in chat. You might need to set up a mute role, " +
														 "but most of the time ModCore can do it for you."),
		 RequirePermissions(Permissions.MuteMembers)]
		public async Task TempMuteAsync(CommandContext ctx, [Description("Member to temporarily mute")]DiscordMember m,
			[Description("How long this member will be muted")]TimeSpan ts, [Description("Reason to temp mute this member")]string reason = "")
		{
			if (ctx.Member.Id == m.Id)
			{
				await ctx.SafeRespondAsync("You can't do that to yourself! You have so much to live for!");
				return;
			}

			var guildSettings = ctx.GetGuildSettings() ?? new GuildSettings();
			if (guildSettings == null)
			{
				await ctx.SafeRespondAsync(
					"Guild is not configured. Adjust this guild's configuration and re-run this command.");
				return;
			}

			var b = guildSettings.MuteRoleId;
			var mute = ctx.Guild.GetRole(b);
			if (b == 0 || mute == null)
			{
				var setupStatus = await Utils.SetupMuteRole(ctx.Guild, ctx.Member, m);
				mute = setupStatus.Role;
				guildSettings.MuteRoleId = setupStatus.Role.Id;
				await ctx.SafeRespondAsync("Mute role is not configured or missing, " + setupStatus.Message);
				await ctx.SetGuildSettingsAsync(guildSettings);
			}
			await Utils.GuaranteeMuteRoleDeniedEverywhere(ctx.Guild, mute);

			var timer = Timers.FindNearestTimer(TimerActionType.Unmute, m.Id, 0, ctx.Guild.Id, this.Database);
			if (timer != null)
			{
				await ctx.SafeRespondAsync("This member was already muted! Please try to unmute them first!");
				return;
			}

			var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
			var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
			await m.ElevatedMessageAsync($"You've been temporarily muted in {ctx.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the following reason:\n```\n{reason}\n```")}" +
				$"\nYou can talk again after {ts.Humanize(4, minUnit: TimeUnit.Second)}");
			await m.GrantRoleAsync(mute, $"{ustr}{rstr} (mute)");
			// Add timer
			var now = DateTimeOffset.UtcNow;
			var dispatchAt = now + ts;

			var reminder = new DatabaseTimer
			{
				GuildId = (long)ctx.Guild.Id,
				ChannelId = 0,
				UserId = (long)m.Id,
				DispatchAt = dispatchAt.LocalDateTime,
				ActionType = TimerActionType.Unmute
			};
			reminder.SetData(new TimerUnmuteData
			{
				Discriminator = m.Discriminator,
				DisplayName = m.Username,
				UserId = (long)m.Id,
				MuteRoleId = (long)(ctx.GetGuildSettings() ?? new GuildSettings()).MuteRoleId
			});
			using (var db = this.Database.CreateContext())
			{
				db.Timers.Add(reminder);
				await db.SaveChangesAsync();
			}

			Timers.RescheduleTimers(ctx.Client, this.Database, this.Shared);

			// End of Timer adding
			await ctx.SafeRespondAsync(
				$"Tempmuted user {m.DisplayName} (ID:{m.Id}) to be unmuted in {ts.Humanize(4, minUnit: TimeUnit.Second)}");

			await ctx.LogActionAsync(
				$"Tempmuted user {m.DisplayName} (ID:{m.Id}) to be unmuted in {ts.Humanize(4, minUnit: TimeUnit.Second)}");
		}

		[Command("schedulepin"), Aliases("sp"), Description("Schedules a pinned message. _I really don't know why " +
															"you'd want to do this._"),
		 RequirePermissions(Permissions.ManageMessages)]
		public async Task SchedulePinAsync(CommandContext ctx, [Description("Message to schedule a pin for")]DiscordMessage message,
			[Description("How long it will take for this message to get pinned")]TimeSpan pinfrom)
		{
			// Add timer
			var now = DateTimeOffset.UtcNow;
			var dispatchAt = now + pinfrom;

			var reminder = new DatabaseTimer
			{
				GuildId = (long)ctx.Guild.Id,
				ChannelId = (long)ctx.Channel.Id,
				UserId = (long)ctx.User.Id,
				DispatchAt = dispatchAt.LocalDateTime,
				ActionType = TimerActionType.Pin
			};
			reminder.SetData(new TimerPinData { MessageId = (long)message.Id, ChannelId = (long)ctx.Channel.Id });
			using (var db = this.Database.CreateContext())
			{
				db.Timers.Add(reminder);
				await db.SaveChangesAsync();
			}

			Timers.RescheduleTimers(ctx.Client, this.Database, this.Shared);

			// End of Timer adding
			await ctx.SafeRespondAsync(
				$"After {pinfrom.Humanize(4, minUnit: TimeUnit.Second)} this message will be pinned");
		}

		[Command("scheduleunpin"), Aliases("sup"), Description("Schedules unpinning a pinned message. This command " +
															   "really is useless, isn't it?"),
		 RequirePermissions(Permissions.ManageMessages)]
		public async Task ScheduleUnpinAsync(CommandContext ctx, [Description("Message to schedule unpinning for")]DiscordMessage message,
			[Description("Time it will take before this message gets unpinned")]TimeSpan pinuntil)
		{
			if (!message.Pinned)
				await message.PinAsync();
			// Add timer
			var now = DateTimeOffset.UtcNow;
			var dispatchAt = now + pinuntil;

			var reminder = new DatabaseTimer
			{
				GuildId = (long)ctx.Guild.Id,
				ChannelId = (long)ctx.Channel.Id,
				UserId = (long)ctx.User.Id,
				DispatchAt = dispatchAt.LocalDateTime,
				ActionType = TimerActionType.Unpin
			};
			reminder.SetData(new TimerUnpinData { MessageId = (long)message.Id, ChannelId = (long)ctx.Channel.Id });
			using (var db = this.Database.CreateContext())
			{
				db.Timers.Add(reminder);
				await db.SaveChangesAsync();
			}

			Timers.RescheduleTimers(ctx.Client, this.Database, this.Shared);

			// End of Timer adding
			await ctx.SafeRespondAsync(
				$"In {pinuntil.Humanize(4, minUnit: TimeUnit.Second)} this message will be unpinned.");
		}

		[Command("listbans"), Aliases("lb"), Description("Lists banned users. Real complex stuff."), RequireUserPermissions(Permissions.ViewAuditLog)]
		public async Task ListBansAsync(CommandContext ctx)
		{
			var bans = await ctx.Guild.GetBansAsync();
			if (bans.Count == 0)
			{
				await ctx.SafeRespondAsync("No user is banned.");
				return;
			}

			var interactivity = this.Interactivity;
			var page = 1;
			var total = bans.Count / 10 + (bans.Count % 10 == 0 ? 0 : 1);
			var pages = new List<Page>();
			var cembed = new DiscordEmbedBuilder
			{
				Title = "Banned users:",
				Footer = new DiscordEmbedBuilder.EmbedFooter
				{
					Text = $"Page {page} of {total}"
				}
			};
			foreach (var xr in bans)
			{
				var user = xr.User;
				var reason = (string.IsNullOrWhiteSpace(xr.Reason) ? "No reason given." : xr.Reason);
				cembed.AddField(
					$"{user.Username}#{user.Discriminator} (ID: {user.Id})",
					$"{reason}");
				if (cembed.Fields.Count < 10) continue;
				page++;
				pages.Add(new Page { Embed = cembed.Build() });
				cembed = new DiscordEmbedBuilder
				{
					Title = "Banned users",
					Footer = new DiscordEmbedBuilder.EmbedFooter
					{
						Text = $"Page {page} of {total}"
					}
				};
			}
			if (cembed.Fields.Count > 0)
				pages.Add(new Page { Embed = cembed.Build() });

			if (pages.Count > 1)
				await interactivity.SendPaginatedMessage(ctx.Channel, ctx.User, pages);
			else
				await ctx.ElevatedRespondAsync(embed: pages.First().Embed);
		}
		[Group("selfrole"), Description("Commands to give or take selfroles."), RequireBotPermissions(Permissions.ManageRoles)]
		public class SelfRole : BaseCommandModule
		{
			private DatabaseContextBuilder Database { get; }

			public SelfRole(DatabaseContextBuilder db)
			{
				this.Database = db;
			}

			[Command("give"), Aliases("g"), Description("Gives the command callee a specified role, if " +
																	 "ModCore has been configured to allow so.")]
			public async Task GiveAsync(CommandContext ctx, [RemainingText, Description("Role you want to give to yourself")] DiscordRole role)
			{
				var cfg = ctx.GetGuildSettings() ?? new GuildSettings(); ;
				if (cfg.SelfRoles.Contains(role.Id))
				{
					if (ctx.Member.Roles.Any(x => x.Id == role.Id))
					{
						await ctx.SafeRespondAsync("You already have that role!");
						return;
					}
					if (ctx.Guild.CurrentMember.Roles.Any(x => x.Position >= role.Position))
					{
						await ctx.Member.GrantRoleAsync(role, "AutoRole granted.");
						await ctx.SafeRespondAsync($"Granted you the role `{role.Name}`.");
					}
					else
						await ctx.SafeRespondAsync("Can't grant you this role because that role is above my highest role!");
				}
				else
				{
					await ctx.SafeRespondAsync("You can't grant yourself that role!");
				}
			}

			[Command("take"), Aliases("t"), Description("Removes a specified role from the command callee, if " +
																	 "ModCore has been configured to allow so.")]
			public async Task TakeAsync(CommandContext ctx, [RemainingText, Description("Role you want to take from yourself")] DiscordRole role)
			{
				var cfg = ctx.GetGuildSettings() ?? new GuildSettings(); ;

				if (cfg.SelfRoles.Contains(role.Id))
				{
					if (ctx.Member.Roles.All(x => x.Id != role.Id))
					{
						await ctx.SafeRespondAsync("You don't have that role!");
						return;
					}
					if (ctx.Guild.CurrentMember.Roles.Any(x => x.Position >= role.Position))
					{
						await ctx.Member.RevokeRoleAsync(role, "AutoRole revoke.");
						await ctx.SafeRespondAsync($"Revoked your role: `{role.Name}`.");
					}
					else
						await ctx.SafeRespondAsync("Can't take this role because that role is above my highest role!");
				}
				else
				{
					await ctx.SafeRespondAsync("You can't revoke that role!");
				}
			}

			[Command("list"), Aliases("l"), Description("Lists all available selfroles, if any.")]
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
					await ctx.SafeRespondAsync("No available selfroles.");
				}
			}
		}

		[Command("announce"), Description("Announces a message to a channel, additionally mentioning a role.")]
		[RequireBotPermissions(Permissions.ManageRoles), RequireUserPermissions(Permissions.MentionEveryone)]
		public async Task AnnounceAsync(CommandContext ctx, [Description("Role to announce for")]DiscordRole role,
			[Description("Channel to announce to")]DiscordChannel channel, [RemainingText, Description("Announcement text")] string message)
		{
			if (!role.IsMentionable)
			{
				await role.UpdateAsync(mentionable: true);
				await channel.SafeMessageAsync($"{role.Mention} {message}", ctx);
				await role.UpdateAsync(mentionable: false);
				await ctx.Message.DeleteAsync();
				await ctx.LogActionAsync($"Announced {message}\nTo channel: #{channel.Name}\nTo role: {role.Name}");
			}
			else
			{
				await ctx.Channel.SafeMessageAsync("You can't announce to that role because it is mentionable!", true);
				await ctx.LogActionAsync(
					$"Failed announcement\nMessage: {message}\nTo channel: #{channel.Name}\nTo role: {role.Name}");
			}
		}

		[Command("poll"), Description("Creates a reaction-based poll.")]
		[RequireBotPermissions(Permissions.ManageMessages)]
		public async Task PollAsync(CommandContext ctx, [Description("Question to ask")]string message, [Description("Reaction options")]params DiscordEmoji[] options)
		{
			await ctx.Message.DeleteAsync();
			var m = await ctx.SafeRespondAsync($"**[Poll]**: {message}");
			var intr = ctx.Client.GetInteractivity();
			var responses = await intr.CreatePollAsync(m, options);
			StringBuilder sb = new StringBuilder($"**[Poll]**: {message}");
			foreach (var em in options)
			{
				sb.AppendLine();
				sb.Append(em.ToString() + ": ");
				if (responses.Reactions.Keys.Contains(em))
					sb.Append(responses.Reactions[em]);
				else
					sb.Append(0);
			}
			await m.DeleteAllReactionsAsync("Remove polling reactions");
			await ctx.SafeModifyAsync(m, sb.ToString());
		}

		[Command("buildmessage"), Description("Builds a message for you, complete with embeds.")]
		[RequirePermissions(Permissions.ManageMessages)]
		public async Task BuildMessageAsync(CommandContext ctx)
		{
			var msg = await ctx.RespondAsync($"Hello and welcome to the ModCore message builder!\nI am your host Smirky, and I will guide you through the creation of a message!" +
				$"{DiscordEmoji.FromName(ctx.Client, ":smirk:").ToString()}\n\n___**[REMEMBER: WIP!!]**___");
			await Task.Delay(TimeSpan.FromSeconds(2));
			var menu = new DiscordEmbedBuilder()
				.WithTitle("Message Builder Options")
				.WithDescription("__0__. Set Content\n" +
								 "__1__. Set Title\n" +
								 "__2__. Set Description\n" +
								 "__3__. Set Image\n" +
								 "__4__. Set Thumbnail\n" +
								 "__5__. Set Timestamp\n" +
								 "__6__. Set Url\n" +
								 "__7__. Set Color\n" +
								 "__8__. Set Footer\n" +
								 "__9__. Set Author\n" +
								 "__10__. Add Field\n" +
								 "__11__. Clear fields\n" +
								 "__12__. Send message.\n" +
								 "__13__. Preview message.\n\n" +
								 "__14__. Cancel"
								 )
				.Build();

			var building = true;
			var embed = new DiscordEmbedBuilder();
			string content = "\u200B";

			while (building)
			{
				await msg.ModifyAsync("", menu);
				var response = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
				if (response == null)
				{
					await msg.ModifyAsync("Message building timed out.", null);
					return;
				}

				await response.Message.DeleteAsync();

				switch (response.Message.Content.ToLower())
				{
					default:
						#region Invalid Response
						await msg.ModifyAsync("Invalid response.", null);
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "0":
						#region Set Content
						await msg.ModifyAsync("What would you like to set the Content to?", null);
						var case0 = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (case0?.Message?.Content != null)
						{
							content = case0.Message.Content;
							await case0.Message.DeleteAsync();
							await msg.ModifyAsync($"Content set.", null);
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "1":
						#region Set Title
						await msg.ModifyAsync("What would you like to set the Title to?", null);
						var case1 = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (case1?.Message?.Content != null)
						{
							embed.WithTitle(case1.Message.Content);
							await case1.Message.DeleteAsync();
							await msg.ModifyAsync($"Title set.", null);
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "2":
						#region Set Description
						await msg.ModifyAsync("What would you like to set the Description to?", null);
						var case2 = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (case2?.Message?.Content != null)
						{
							embed.WithDescription(case2.Message.Content);
							await case2.Message.DeleteAsync();
							await msg.ModifyAsync($"Description set.", null);
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "3":
						#region Set Image
						await msg.ModifyAsync("What would you like to set the Image URL to?", null);
						var case3 = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (case3?.Message?.Content != null)
						{
							embed.WithImageUrl(case3.Message.Content);
							await case3.Message.DeleteAsync();
							await msg.ModifyAsync($"Image URL set.", null);
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "4":
						#region Set Thumbnail
						await msg.ModifyAsync("What would you like to set the Thumbnail Image URL to?", null);
						var case4 = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (case4?.Message?.Content != null)
						{
							embed.WithThumbnailUrl(case4.Message.Content);
							await case4.Message.DeleteAsync();
							await msg.ModifyAsync($"Thumbnail Image URL set.", null);
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "5":
						#region Set Timestamp
						// Do some fancy pancy timestamp parsing
						await msg.ModifyAsync("What would you like to set the Timestamp to?", null);
						var case5 = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (case5?.Message?.Content != null)
						{
							var ts = await new DateTimeOffsetConverter().ConvertAsync(case5.Message.Content, ctx);
							if (ts.HasValue)
							{
								embed.WithTimestamp(ts.Value);
								await case5.Message.DeleteAsync();
								await msg.ModifyAsync($"Timestamp set.", null);
							}
							else
							{
								await case5.Message.DeleteAsync();
								await msg.ModifyAsync($"Invalid timestamp.", null);
							}
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "6":
						#region Set Url
						await msg.ModifyAsync("What would you like to set the URL to?", null);
						var case6 = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (case6?.Message?.Content != null)
						{
							embed.WithUrl(case6.Message.Content);
							await case6.Message.DeleteAsync();
							await msg.ModifyAsync($"URL set.", null);
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "7":
						#region Set Color
						await msg.ModifyAsync("What would you like to set the Color to? (HTML)", null);
						var case7 = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (case7?.Message?.Content != null)
						{
							embed.WithColor(new DiscordColor(case7.Message.Content));
							await case7.Message.DeleteAsync();
							await msg.ModifyAsync($"Color set.", null);
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "8":
						#region Set Footer
						await msg.ModifyAsync("Method not yet implemented.", null);
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "9":
						#region Set Author
						await msg.ModifyAsync("Method not yet implemented.", null);
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "10":
						#region Add Field
						await msg.ModifyAsync("Method not yet implemented.", null);
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "11":
						#region Clear Fields
						await msg.ModifyAsync("Method not yet implemented.", null);
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "12":
						#region Send Message
						// Remember to pick a channel to send to first!!
						await msg.ModifyAsync("What channel do you want to send this message to?", null);
						var case12 = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (case12?.Message?.Content != null)
						{
							var dcc = new DiscordChannelConverter();
							var channel = await dcc.ConvertAsync(case12.Message.Content, ctx);
							if (channel.HasValue)
							{
								await channel.Value.SendMessageAsync(content, embed: embed.Build());
								return;
							}
							else
							{
								await msg.ModifyAsync("Invalid channel.", null);
							}
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "13":
						#region Preview Message
						DiscordEmbed preview = null;
						try
						{
							preview = embed.Build();
						}
						catch (Exception)
						{

						}
						await msg.ModifyAsync(content, preview);
						await Task.Delay(TimeSpan.FromSeconds(3));
						break;
					#endregion
					case "14":
						#region Cancel Building
						await msg.ModifyAsync("Message building canceled.", null);
						building = false;
						break;
						#endregion
				}
			}
		}
	}
}
