using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.EventHandling;
using Humanizer;
using Humanizer.Localisation;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Listeners;
using ModCore.Logic;
using ModCore.Logic.Extensions;
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

		[Command("about"), Description("About this bot.")]
		public async Task AboutAsync(CommandContext ctx)
		{
			var eb = new DiscordEmbedBuilder()
				.WithColor(new DiscordColor("#C1272D"))
				.WithTitle("ModCore")
				.WithDescription("A powerful moderating bot written on top of DSharpPlus")
                .AddField("Main developer", "[Naamloos](https://github.com/Naamloos)")
				.AddField("Special thanks to these contributors:",
				    "[uwx](https://github.com/uwx), " +
				    "[jcryer](https://github.com/jcryer), " +
				    "[Emzi0767](https://github.com/Emzi0767), " +
				    "[YourAverageBlackGuy](https://github.com/YourAverageBlackGuy), " +
				    "[DrCreo](https://github.com/DrCreo), " +
				    "[aexolate](https://github.com/aexolate), " +
                    "[Drake103](https://github.com/Drake103) and " +
                    "[Izumemori](https://github.com/Izumemori)")
                .AddField("Environment", 
                    $"*OS:* {Environment.OSVersion.VersionString}" +
                    $"\n*Framework:* {Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName}" +
                    $"\n*DSharpPlus:* {ctx.Client.VersionString}" +
                    $"\n*Servers:* {this.Shared.ModCore.Shards.Select(x => x.Client.Guilds.Count).Sum()}" +
                    $"\n*Shards:* {this.Shared.ModCore.Shards.Count}")
				.AddField("Contribute?", "Contributions are always welcome at our [GitHub repo.](https://github.com/Naamloos/ModCore)")
				.WithThumbnailUrl(ctx.Client.CurrentUser.AvatarUrl)
				.Build();

			await ctx.ElevatedRespondAsync(embed: eb);
		}

		[Command("ping"), Description("Check ModCore's API connection status."), CheckDisable]
		public async Task PingAsync(CommandContext ctx)
		{
			await ctx.SafeRespondAsync($"Pong: ({ctx.Client.Ping}) ms.");
		}

		[Command("prefix"), Description("Check ModCore's current prefix."), CheckDisable]
		public async Task PrefixAsync(CommandContext ctx)
		{
			await ctx.IfGuildSettings(
				async (e) => await ctx.SafeRespondAsync($"Current prefix: {e.Prefix}"),
				async () => await ctx.SafeRespondAsync($"Current prefix: {this.Shared.DefaultPrefix}"));
		}

		[Command("uptime"), Description("Check ModCore's uptime."), Aliases("u"), CheckDisable]
		public async Task UptimeAsync(CommandContext ctx)
		{
			var st = this.StartTimes;
			var bup = DateTimeOffset.Now.Subtract(st.ProcessStartTime);
			var sup = DateTimeOffset.Now.Subtract(st.SocketStartTime);

			// Needs improvement
			await ctx.SafeRespondUnformattedAsync(
				$"Program uptime: {string.Format("{0} days, {1}", bup.ToString("dd"), bup.ToString(@"hh\:mm\:ss"))}\n" +
				$"Socket uptime: {string.Format("{0} days, {1}", sup.ToString("dd"), sup.ToString(@"hh\:mm\:ss"))}");
		}

		[Command("invite"), Description("Get an invite to this ModCore instance. Sharing is caring!"), Aliases("inv"), CheckDisable]
		public async Task InviteAsync(CommandContext ctx)
		{
			//TODO replace with a link to a nice invite builder!
			// what the hell is an invite builder? - chris
			var app = ctx.Client.CurrentApplication;
			if (app.IsPublic != null && (bool)app.IsPublic)
				await ctx.SafeRespondAsync(
					$"Add ModCore to your server!\n<https://modcore.naamloos.dev/info/invite>");
			else
				await ctx.SafeRespondUnformattedAsync("I'm sorry Mario, but this instance of ModCore has been set to private!");
		}

		[Command("ban"), Description("Bans a member."), Aliases("b"), RequirePermissions(Permissions.BanMembers), CheckDisable]
		public async Task BanAsync(CommandContext ctx, [Description("Member to ban")] DiscordMember m,
			[RemainingText, Description("Reason to ban this member")] string reason = "")
		{
			if (ctx.Member.Id == m.Id)
			{
				await ctx.SafeRespondUnformattedAsync("You can't do that to yourself! You have so much to live for!");
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
		 Aliases("hb"), RequirePermissions(Permissions.BanMembers), CheckDisable]
		public async Task HackBanAsync(CommandContext ctx, [Description("ID of user to ban")]ulong id,
			[RemainingText, Description("Reason to ban this member")] string reason = "")
		{
			if (ctx.Member.Id == id)
			{
				await ctx.SafeRespondUnformattedAsync("You can't do that to yourself! You have so much to live for!");
				return;
			}

			var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
			var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
			await ctx.Guild.BanMemberAsync(id, 7, $"{ustr}{rstr}");
			await ctx.SafeRespondUnformattedAsync("User hackbanned successfully.");

			await ctx.LogActionAsync($"Hackbanned ID: {id}\n{rstr}");
		}

		[Command("kick"), Description("Kicks a member from the guild. Can optionally provide a reason for kick."),
		 Aliases("k"), RequirePermissions(Permissions.KickMembers), CheckDisable]
		public async Task KickAsync(CommandContext ctx, [Description("Member to kick")]DiscordMember m,
			[RemainingText, Description("Reason to kick this member")] string reason = "")
		{
			if (ctx.Member.Id == m.Id)
			{
				await ctx.SafeRespondUnformattedAsync("You can't do that to yourself! You have so much to live for!");
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
		 RequireUserPermissions(Permissions.KickMembers), RequireBotPermissions(Permissions.BanMembers), CheckDisable]
		public async Task SoftbanAsync(CommandContext ctx, [Description("Member to softban")]DiscordMember m,
			[RemainingText, Description("Reason to softban this member")] string reason = "")
		{
			if (ctx.Member.Id == m.Id)
			{
				await ctx.SafeRespondUnformattedAsync("You can't do that to yourself! You have so much to live for!");
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

		[Command("mute"), Description("Mutes an user indefinitely. This will prevent them from speaking in chat. " +
									  "You might need to set up a mute role, but most of the time ModCore can do it " +
									  "for you."), Aliases("m"), RequirePermissions(Permissions.MuteMembers),
		 RequireBotPermissions(Permissions.ManageRoles), CheckDisable]
		public async Task MuteAsync(CommandContext ctx, [Description("Member to mute")]DiscordMember m,
			[RemainingText, Description("Reason to mute this member")] string reason = "")
		{
			if (ctx.Member.Id == m.Id)
			{
				await ctx.SafeRespondUnformattedAsync("You can't do that to yourself! You have so much to live for!");
				return;
			}

			var guildSettings = ctx.GetGuildSettings() ?? new GuildSettings();
			if (guildSettings == null)
			{
				await ctx.SafeRespondUnformattedAsync("Guild is not configured, please configure and rerun");
				return;
			}

			var b = guildSettings.MuteRoleId;
			var mute = ctx.Guild.GetRole(b);
			if (b == 0 || mute == null)
			{
				var (Role, Message) = await Utils.SetupMuteRole(ctx.Guild, ctx.Member, m);
				mute = Role;
				guildSettings.MuteRoleId = Role.Id;
				await ctx.SafeRespondUnformattedAsync("Mute role is not configured or missing, " + Message);
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
		 RequireBotPermissions(Permissions.ManageRoles), CheckDisable]
		public async Task UnmuteAsync(CommandContext ctx, [Description("Member to unmute")]DiscordMember m,
			[RemainingText, Description("Reason to unmute this member")] string reason = "")
		{
			if (ctx.Member.Id == m.Id)
			{
				await ctx.SafeRespondUnformattedAsync("You can't do that to yourself! You have so much to live for!");
				return;
			}

			var gcfg = ctx.GetGuildSettings() ?? new GuildSettings();
			if (gcfg == null)
			{
				await ctx.SafeRespondUnformattedAsync(
					"Guild is not configured. Adjust this guild's configuration and re-run this command.");
				return;
			}

			var b = gcfg.MuteRoleId;
			var mute = ctx.Guild.GetRole(b);
			if (b == 0 || mute == null)
			{
				await ctx.SafeRespondUnformattedAsync(
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
		 RequireUserPermissions(Permissions.Administrator), CheckDisable]
		public async Task LeaveAsync(CommandContext ctx)
		{
			var interactivity = this.Interactivity;
			await ctx.SafeRespondUnformattedAsync("Are you sure you want to remove modcore from your guild?");
			var m = await interactivity.WaitForMessageAsync(
				x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Member.Id, TimeSpan.FromSeconds(30));

			if (m.Result == null)
				await ctx.SafeRespondUnformattedAsync("Timed out.");
			else if (m.Result.Content.ToLowerInvariant() == "yes")
			{
				await ctx.SafeRespondUnformattedAsync("Thanks for using ModCore. Leaving this guild.");
				await ctx.LogActionAsync("Left your server. Thanks for using ModCore.");
				await ctx.Guild.LeaveAsync();
			}
			else
				await ctx.SafeRespondUnformattedAsync("Operation canceled by user.");
		}

		[Command("tempban"), Aliases("tb"), Description(
			 "Temporarily bans a member. They will be automatically unbanned " +
			 "after a set amount of time."),
		 RequirePermissions(Permissions.BanMembers), CheckDisable]
		public async Task TempBanAsync(CommandContext ctx, [Description("Member to ban temporarily")]DiscordMember m,
			[Description("How long this member will be banned")]TimeSpan ts, [Description("Why this member got banned")]string reason = "")
		{
			if (ctx.Member.Id == m.Id)
			{
				await ctx.SafeRespondUnformattedAsync("You can't do that to yourself! You have so much to live for!");
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

			await Timers.RescheduleTimers(ctx.Client, this.Database, this.Shared);

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
		 RequirePermissions(Permissions.MuteMembers), CheckDisable]
		public async Task TempMuteAsync(CommandContext ctx, [Description("Member to temporarily mute")]DiscordMember m,
			[Description("How long this member will be muted")]TimeSpan ts, [Description("Reason to temp mute this member")]string reason = "")
		{
			if (ctx.Member.Id == m.Id)
			{
				await ctx.SafeRespondUnformattedAsync("You can't do that to yourself! You have so much to live for!");
				return;
			}

			var guildSettings = ctx.GetGuildSettings() ?? new GuildSettings();
			if (guildSettings == null)
			{
				await ctx.SafeRespondUnformattedAsync(
					"Guild is not configured. Adjust this guild's configuration and re-run this command.");
				return;
			}

			var b = guildSettings.MuteRoleId;
			var mute = ctx.Guild.GetRole(b);
			if (b == 0 || mute == null)
			{
				var (Role, Message) = await Utils.SetupMuteRole(ctx.Guild, ctx.Member, m);
				mute = Role;
				guildSettings.MuteRoleId = Role.Id;
				await ctx.SafeRespondUnformattedAsync("Mute role is not configured or missing, " + Message);
				await ctx.SetGuildSettingsAsync(guildSettings);
			}
			await Utils.GuaranteeMuteRoleDeniedEverywhere(ctx.Guild, mute);

			var timer = Timers.FindNearestTimer(TimerActionType.Unmute, m.Id, 0, ctx.Guild.Id, this.Database);
			if (timer != null)
			{
				await ctx.SafeRespondUnformattedAsync("This member was already muted! Please try to unmute them first!");
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

			await Timers.RescheduleTimers(ctx.Client, this.Database, this.Shared);

			// End of Timer adding
			await ctx.SafeRespondAsync(
				$"Tempmuted user {m.DisplayName} (ID:{m.Id}) to be unmuted in {ts.Humanize(4, minUnit: TimeUnit.Second)}");

			await ctx.LogActionAsync(
				$"Tempmuted user {m.DisplayName} (ID:{m.Id}) to be unmuted in {ts.Humanize(4, minUnit: TimeUnit.Second)}");
		}

		[Command("schedulepin"), Aliases("sp"), Description("Schedules a pinned message. _I really don't know why " +
															"you'd want to do this._"),
		 RequirePermissions(Permissions.ManageMessages), CheckDisable]
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

			await Timers.RescheduleTimers(ctx.Client, this.Database, this.Shared);

			// End of Timer adding
			await ctx.SafeRespondAsync(
				$"After {pinfrom.Humanize(4, minUnit: TimeUnit.Second)} this message will be pinned");
		}

		[Command("scheduleunpin"), Aliases("sup"), Description("Schedules unpinning a pinned message. This command " +
															   "really is useless, isn't it?"),
		 RequirePermissions(Permissions.ManageMessages), CheckDisable]
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

			await Timers.RescheduleTimers(ctx.Client, this.Database, this.Shared);

			// End of Timer adding
			await ctx.SafeRespondAsync(
				$"In {pinuntil.Humanize(4, minUnit: TimeUnit.Second)} this message will be unpinned.");
		}

		[Command("listbans"), Aliases("lb"), Description("Lists banned users. Real complex stuff."), RequireUserPermissions(Permissions.ViewAuditLog), CheckDisable]
		[RequireBotPermissions(Permissions.BanMembers)]
        public async Task ListBansAsync(CommandContext ctx)
		{
			var bans = await ctx.Guild.GetBansAsync();
			if (bans.Count == 0)
			{
				await ctx.SafeRespondUnformattedAsync("No user is banned.");
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
				pages.Add(new Page("", cembed));
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
				pages.Add(new Page("", cembed));

			if (pages.Count > 1)
				await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages.ToArray(), new PaginationEmojis());
			else
				await ctx.ElevatedRespondAsync(embed: pages.First().Embed);
		}

		[Command("announce"), Description("Announces a message to a channel, additionally mentioning a role.")]
		[RequireBotPermissions(Permissions.ManageRoles), RequireUserPermissions(Permissions.MentionEveryone), CheckDisable]
		public async Task AnnounceAsync(CommandContext ctx, [Description("Role to announce for")]DiscordRole role,
			[Description("Channel to announce to")]DiscordChannel channel, [RemainingText, Description("Announcement text")] string message)
		{
			if (!role.IsMentionable)
			{
				await role.ModifyAsync(mentionable: true);
				await channel.SafeMessageAsync($"{role.Mention} {message}", ctx);
				await role.ModifyAsync(mentionable: false);
				await ctx.Message.DeleteAsync();
				await ctx.LogActionAsync($"Announced {message}\nTo channel: #{channel.Name}\nTo role: {role.Name}");
			}
			else
			{
				await ctx.Channel.SafeMessageUnformattedAsync("You can't announce to that role because it is mentionable!", true);
				await ctx.LogActionAsync(
					$"Failed announcement\nMessage: {message}\nTo channel: #{channel.Name}\nTo role: {role.Name}");
			}
		}

		[Command("poll"), Description("Creates a reaction-based poll."), CheckDisable]
		[RequireBotPermissions(Permissions.ManageMessages)]
		public async Task PollAsync(CommandContext ctx, [Description("Question to ask")]string message, [Description("Time to run poll")]TimeSpan timespan, [Description("Reaction options")]params DiscordEmoji[] options)
		{
			await ctx.Message.DeleteAsync();
			var m = await ctx.SafeRespondAsync($"**[Poll]**: {message}");
			var intr = ctx.Client.GetInteractivity();
			var responses = await intr.DoPollAsync(m, options, PollBehaviour.DeleteEmojis, timespan);
			StringBuilder sb = new StringBuilder($"**[Poll]**: {message}");
			foreach (var em in responses)
			{
				sb.AppendLine();
				sb.Append($"{em.Emoji.ToString()}: {em.Total}");
			}
			await ctx.SafeModifyUnformattedAsync(m, sb.ToString());
		}

		[Command("buildmessage"), Description("Builds a message for you, complete with embeds."), CheckDisable]
		[RequirePermissions(Permissions.ManageMessages)]
		public async Task BuildMessageAsync(CommandContext ctx)
		{
			var msg = await ctx.RespondAsync($"Hello and welcome to the ModCore message builder!\nI am your host Smirky, and I will guide you through the creation of a message! "
				+ DiscordEmoji.FromName(ctx.Client, ":smirk:").ToString());
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
								 "__8__. Set Footer text\n" +
								 "__8b__. Set Footer icon\n" +
								 "__9__. Set Author name\n" +
								 "__9b__. Set Author icon\n" +
								 "__9c__. Set Author URL\n" +
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
				if (response.Result == null)
				{
					await msg.ModifyAsync("Message building timed out.", null);
					return;
				}

				await response.Result.DeleteAsync();

				switch (response.Result.Content.ToLower())
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
						if (!case0.TimedOut)
						{
							content = case0.Result.Content;
							await case0.Result.DeleteAsync();
							await msg.ModifyAsync($"Content set.", null);
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "1":
						#region Set Title
						await msg.ModifyAsync("What would you like to set the Title to?", null);
						var case1 = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (!case1.TimedOut)
						{
							embed.WithTitle(case1.Result.Content);
							await case1.Result.DeleteAsync();
							await msg.ModifyAsync($"Title set.", null);
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "2":
						#region Set Description
						await msg.ModifyAsync("What would you like to set the Description to?", null);
						var case2 = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (!case2.TimedOut)
						{
							embed.WithDescription(case2.Result.Content);
							await case2.Result.DeleteAsync();
							await msg.ModifyAsync($"Description set.", null);
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "3":
						#region Set Image
						await msg.ModifyAsync("What would you like to set the Image URL to?", null);
						var case3 = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (!case3.TimedOut)
						{
							embed.WithImageUrl(case3.Result.Content);
							await case3.Result.DeleteAsync();
							await msg.ModifyAsync($"Image URL set.", null);
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "4":
						#region Set Thumbnail
						await msg.ModifyAsync("What would you like to set the Thumbnail Image URL to?", null);
						var case4 = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (!case4.TimedOut)
						{
							embed.WithThumbnailUrl(case4.Result.Content);
							await case4.Result.DeleteAsync();
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
						if (!case5.TimedOut)
						{
                            var dtoc = new DateTimeOffsetConverter() as IArgumentConverter<DateTimeOffset>;
							var ts = await dtoc.ConvertAsync(case5.Result.Content, ctx);
							if (ts.HasValue)
							{
								embed.WithTimestamp(ts.Value);
								await case5.Result.DeleteAsync();
								await msg.ModifyAsync($"Timestamp set.", null);
							}
							else
							{
								await case5.Result.DeleteAsync();
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
						if (!case6.TimedOut)
						{
							embed.WithUrl(case6.Result.Content);
							await case6.Result.DeleteAsync();
							await msg.ModifyAsync($"URL set.", null);
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "7":
						#region Set Color
						await msg.ModifyAsync("What would you like to set the Color to? (HTML)", null);
						var case7 = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (!case7.TimedOut)
						{
							embed.WithColor(new DiscordColor(case7.Result.Content));
							await case7.Result.DeleteAsync();
							await msg.ModifyAsync($"Color set.", null);
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
						#endregion
					case "8":
						#region Set Footer Text
						await msg.ModifyAsync("What would you like to set the Footer text to?", null);
						var case8 = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (!case8.TimedOut)
						{
							if (embed.Footer == null)
								embed.WithFooter(case8.Result.Content);
							else
								embed.Footer.Text = case8.Result.Content;
							await case8.Result.DeleteAsync();
							await msg.ModifyAsync($"Footer text set.", null);
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "8b":
						#region Set Footer Icon
						await msg.ModifyAsync("What would you like to set the Footer icon URL to?", null);
						var case8b = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (!case8b.TimedOut)
						{
							if (embed.Footer == null)
								embed.WithFooter(null, case8b.Result.Content);
							else
								embed.Footer.IconUrl = case8b.Result.Content;

							await case8b.Result.DeleteAsync();
							await msg.ModifyAsync($"Footer icon set.", null);
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "9":
						#region Set Author Name
						await msg.ModifyAsync("What would you like to set the Author name to?", null);
						var case9 = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (!case9.TimedOut)
						{
							if (embed.Author == null)
								embed.WithAuthor(case9.Result.Content);
							else
								embed.Author.Name = case9.Result.Content;
							await case9.Result.DeleteAsync();
							await msg.ModifyAsync($"Author name set.", null);
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "9b":
						#region Set Author Icon
						await msg.ModifyAsync("What would you like to set the Author Icon URL to?", null);
						var case9b = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (!case9b.TimedOut)
						{
							if (embed.Author == null)
								embed.WithAuthor(iconUrl: case9b.Result.Content);
							else
								embed.Author.IconUrl = case9b.Result.Content;
							await case9b.Result.DeleteAsync();
							await msg.ModifyAsync($"Author Icon set.", null);
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "9c":
						#region Set Author Icon
						await msg.ModifyAsync("What would you like to set the Author URL to?", null);
						var case9c = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (!case9c.TimedOut)
						{
							if (embed.Author == null)
								embed.WithAuthor(url: case9c.Result.Content);
							else
								embed.Author.Url = case9c.Result.Content;
							await case9c.Result.DeleteAsync();
							await msg.ModifyAsync($"Author URL set.", null);
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "10":
						#region Add Field
						(string, string, bool) field;

						await msg.ModifyAsync("What should the field title be?", null);
						var case10a = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (case10a.TimedOut)
							break;
						field.Item1 = case10a.Result.Content;
						await case10a.Result.DeleteAsync();

						await msg.ModifyAsync("What should the field content be?", null);
						var case10b = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (case10b.TimedOut)
							break;
						field.Item2 = case10b.Result.Content;
						await case10b.Result.DeleteAsync();

						await msg.ModifyAsync("Should the field be inline?", null);
						var case10c = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (case10c.TimedOut)
							break;
						var bcv = new AugmentedBoolConverter();
						var inl = await bcv.ConvertAsync(case10c.Result.Content, ctx);
						field.Item3 = (inl.HasValue? inl.Value : false);
						await case10c.Result.DeleteAsync();

						embed.AddField(field.Item1, field.Item2, field.Item3);
						await msg.ModifyAsync("Field added.", null);
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
						#endregion
					case "11":
						#region Clear Fields
						embed.ClearFields();
						await msg.ModifyAsync("Cleared fields.", null);
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
						#endregion
					case "12":
						#region Send Message
						// Remember to pick a channel to send to first!!
						await msg.ModifyAsync("What channel do you want to send this message to?", null);
						var case12 = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id);
						if (!case12.TimedOut)
						{
							var dcc = new DiscordChannelConverter() as IArgumentConverter<DiscordChannel>;
							var channel = await dcc.ConvertAsync(case12.Result.Content, ctx);
							if (channel.HasValue)
							{
								await channel.Value.SendMessageAsync(content, embed: embed == new DiscordEmbedBuilder()? embed.Build() : null);
								await case12.Result.DeleteAsync();
								await msg.ModifyAsync("Message sent.", null);
								return;
							}
							else
							{
								await case12.Result.DeleteAsync();
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
							preview = embed == new DiscordEmbedBuilder() ? embed.Build() : null;
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

		[Command("distance")]
		[Description("Counts the amount of messages until a specific Message")]
		public async Task DistanceAsync(CommandContext ctx, DiscordMessage msg)
		{
			if(DateTimeOffset.Now.Subtract(msg.Timestamp).TotalDays > 1)
			{
				await ctx.RespondAsync("Yeah.. Can't do that for messages older than a day");
				return;
			}

			var ms = new List<ulong>();
			while (!ms.Contains(ctx.Message.Id))
			{
				var m = await ctx.Channel.GetMessagesAfterAsync(msg.Id, 100);
				foreach(var mm in m)
				{
					if (!ms.Contains(mm.Id))
						ms.Add(mm.Id);
				}
			}

			await ctx.RespondAsync($"Counted {ms.Count} Messages.");
		}

		[Command("quote")]
		[Description("Quotes a message")]
		public async Task QuoteAsync(CommandContext ctx, DiscordChannel channel, ulong message)
		{
			var m = await channel.GetMessageAsync(message);
			await QuoteAsync(ctx, m);
		}

		[Command("quote")]
		[Description("Quotes a message")]
		public async Task QuoteAsync(CommandContext ctx, DiscordMessage message)
		{
			var embed = new DiscordEmbedBuilder()
				.WithTitle($"Message by {message.Author.Username}#{message.Author.Discriminator}")
				.WithDescription($"{message.Content}\n\n[{this.Shared.Emojis.JumpLink.ToString()}](https://discordapp.com/channels/{message.Channel.GuildId}/{message.ChannelId}/{message.Id})")
				.WithFooter($" Quoted by {ctx.Member.Username}#{ctx.Member.Discriminator}. ID: {message.Id}.", ctx.Member.GetAvatarUrl(ImageFormat.Png))
				.WithThumbnailUrl(message.Author.GetAvatarUrl(ImageFormat.Png))
				.WithTimestamp(message.Timestamp)
				.Build();

			await ctx.Message.DeleteAsync();
			await ctx.RespondAsync(embed: embed);
		}

		[Command("nick")]
		#if !DEBUG
		[Cooldown(1, 60, CooldownBucketType.User)]
		#endif
		[Description("Requests a nickname change to the server staff")]
		public async Task RequestNicknameChangeAsync(CommandContext ctx, [RemainingText] string nick)
		{
			if (nick == ctx.Member.Nickname)
			{
				await ctx.ElevatedRespondAsync("That's already your nickname.");
				return;
			}
			if (nick == ctx.Member.Username)
			{
				await ctx.ElevatedRespondAsync("That's already your username.");
				return;
			}
			
			var yes = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");
			var no = DiscordEmoji.FromName(ctx.Client, ":negative_squared_cross_mark:");

			// attempt to automatically change the person's nickname if they can already change it on their own,
			// and prompt them if we're not able to
			if (ctx.Member.PermissionsIn(ctx.Channel).HasPermission(Permissions.ChangeNickname))
			{
				if (ctx.Guild.CurrentMember.PermissionsIn(ctx.Channel).HasPermission(Permissions.ManageNicknames) && 
				    ctx.Guild.CurrentMember.CanInteract(ctx.Member))
				{
					await ctx.Member.ModifyAsync(member =>
					{
						member.Nickname = nick;
						member.AuditLogReason = "Nickname change requested by " +
						                        "@{reaction.User.Username}#{reaction.User.Discriminator} auto approved " +
						                        "since they already have the Change Nickname permission";
					});
					await ctx.Message.CreateReactionAsync(yes);
					return;
				}

				await ctx.ElevatedRespondAsync("Do it yourself, you have the permission!");
				return;
			}

			await ctx.WithGuildSettings(async cfg =>
			{
				// don't change the member's nickname here, as that violates the hierarchy of permissions
				if (!cfg.RequireNicknameChangeConfirmation)
				{
					if (ctx.Member == ctx.Guild.Owner)
					{
						await ctx.ElevatedRespondAsync("Use the `config nickchange enable` command to enable nickname " +
						                               "change requests.");
					}
					else
					{
						await ctx.ElevatedRespondAsync("The server owner has disabled nickname changing on this server.");
					}

					return;
				}
				
				// only say it's unable to process if BOTH the confirmation requirement is enabled and the bot doesn't
				// have the permissions for it
				if (!ctx.Guild.CurrentMember.PermissionsIn(ctx.Channel).HasPermission(Permissions.ManageNicknames) ||
				    !ctx.Guild.CurrentMember.CanInteract(ctx.Member))
				{
					await ctx.ElevatedRespondAsync("Unable to process nickname change because the bot lacks the " +
					                               "required permissions, or cannot action on this member.");
					return;
				}
				
				var message = await ctx.Guild.GetChannel(cfg.NicknameChangeConfirmationChannel)
					.SendMessageAsync(embed: new DiscordEmbedBuilder()
                    .WithTitle("Nickname change confirmation")
                    .WithDescription($"Member {ctx.Member.Mention} ({ctx.Member.Id}) wants to change their nickname to " +
                            $"{Formatter.Sanitize(nick)}.")
                    .WithFooter("This message will self-destruct in 2 hours."));

				// d#+ nightlies mean we can do this now, and hopefully it won't crash
				await Task.WhenAll(message.CreateReactionAsync(yes), message.CreateReactionAsync(no));

				await ctx.ElevatedRespondAsync(
					"Your request to change username was placed, and should be actioned shortly.");
				
                // TODO: y'all gotta mess with dem timeout shit, else we'll get sum nasty errors
				var res = await this.Interactivity.WaitForReactionAsync(
					e => (e.Emoji == yes || e.Emoji == no) && e.Message == message, timeoutoverride: TimeSpan.FromHours(2));
                var reaction = res.Result;

				if (reaction.Emoji == yes)
				{
					await ctx.Member.ModifyAsync(member => member.Nickname = nick);

					await message.DeleteAsync(
						$"Request to change username accepted by @{reaction.User.Username}#{reaction.User.Discriminator}");
					
					await ctx.Member.SendMessageAsync($"Your name in {ctx.Guild.Name} was successfully changed to " +
					                                  $"{Formatter.Sanitize(nick)}.");
					
					try
					{
						await ctx.Message.CreateReactionAsync(yes);
					} catch { /* empty, message has been deleted */ }
				}
				else
				{
					await message.DeleteAsync(
						$"Request to change username denied by @{reaction.User.Username}#{reaction.User.Discriminator}");
					
					await ctx.Member.SendMessageAsync(
						$"Your request to change your username in {ctx.Guild.Name} was denied.");
					
					try
					{
						await ctx.Message.CreateReactionAsync(no);
					} catch { /* empty, message has been deleted */ }
					
				}

			});
		}

		// TODO: look into why strawpoll errors with a valid payload??
		#if DEBUG
		[Command("strawpoll")]
		[Description("Creates a strawpoll")]
		[RequireBotPermissions(Permissions.ManageMessages)]
		[Cooldown(100, 3600/*strawpoll ratelimit 100 per 60 min*/, CooldownBucketType.Global)]
		public async Task StrawpollAsync(CommandContext ctx, string title, params string[] options)
		{
			await ctx.Message.DeleteAsync();
			var poll = await this.Shared.Strawpoll.CreatePollAsync(title, options);
			await ctx.RespondAsync($"{title}:\n{poll}");
		}
		#endif

		// TODO: use database timer system??
		// TODO: multiple winners
		[Command("giveaway")]
		[Description("Creates a giveaway")]
		[RequireUserPermissions(Permissions.ManageGuild)]
		[RequireBotPermissions(Permissions.ManageMessages | Permissions.AddReactions)]
		[Cooldown(1, 600, CooldownBucketType.Channel)]
		public async Task GiveawayAsync(CommandContext ctx, string prize, TimeSpan time_alive)
		{
			await ctx.Message.DeleteAsync();
			var trophy = DiscordEmoji.FromName(ctx.Client, ":trophy:");
			var gaw = await ctx.RespondAsync($"Hey! {ctx.Member.Mention} is giving away {prize}!\nReact with {trophy.ToString()} to join in!");
			await gaw.CreateReactionAsync(trophy);

			await Task.Delay(time_alive);

			var members = (await gaw.GetReactionsAsync(trophy)).ToList();
			members.RemoveAll(x => x.Id == ctx.Client.CurrentUser.Id);

			var winnerindex = new Random().Next(0, members.Count() - 1);
			var winner = members[winnerindex];

			var tada = DiscordEmoji.FromName(ctx.Client, ":tada:");
			await gaw.ModifyAsync($"{tada.ToString()}{tada.ToString()} " +
				$"{winner.Mention}, you won! Contact {ctx.Member.Mention} for your price! " +
				$"{trophy.ToString()}{trophy.ToString()}");
		}

        [Command("exec")]
        [Description("Executes (multiple) commands")]
        public async Task ExecAsync(CommandContext ctx, [RemainingText]string cmds)
        {
            // TODO big ol testes
            string splitter = @"(?!\\);";

            var split = Regex.Split(cmds, splitter);
            foreach (var s in split)
            {
                var p = ctx.GetGuildSettings()?.Prefix ?? this.Shared.DefaultPrefix;
                //await ctx.CommandsNext.SudoAsync(ctx.User, ctx.Channel, $"{p}{}");
                var cmdraw = s.Replace("\\;", ";").Split(' ').ToList();
                var cmd = cmdraw[0];
                cmdraw.RemoveAt(0);
                var args = String.Join(' ', cmdraw);

                var cmdobj = ctx.CommandsNext.FindCommand(cmd, out args);

                var fctx = ctx.CommandsNext.CreateFakeContext(ctx.User, ctx.Channel, s.Replace("\\;", ";"), p, cmdobj);
                await ctx.CommandsNext.ExecuteCommandAsync(fctx);
            }
        }

        [Command("snipe")]
        [Description("Snipes last deleted message")]
        public async Task SnipeAsync(CommandContext ctx)
        {
            if (this.Shared.DeletedMessages.ContainsKey(ctx.Channel.Id))
            {
                var m = this.Shared.DeletedMessages[ctx.Channel.Id];

                var content = m.Content;
                if (content.Length > 500)
                    content = content.Substring(0, 500) + "...";

                var embed = new DiscordEmbedBuilder().WithAuthor($"{m.Author.Username}#{m.Author.Discriminator}", iconUrl: m.Author.GetAvatarUrl(ImageFormat.Png));

                if(!string.IsNullOrEmpty(m.Content))
                    embed.WithDescription(m.Content);

                await ctx.RespondAsync(embed: embed);
                return;
            }
            await ctx.RespondAsync("No message to snipe!");
        }

        [Command("cooldown")]
        [Description("Sets a custom message cooldown")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task CooldownAsync(CommandContext ctx, int cooldown)
        {
            if (cooldown <= 21600 && cooldown >= 0)
            {
                await ctx.Channel.ModifyAsync(x => x.PerUserRateLimit = cooldown);
                await ctx.RespondAsync($"Set cooldown to {cooldown} seconds.");
                return;
            }
            await ctx.RespondAsync($"Invalid cooldown: {cooldown}");
        }
    }
}
