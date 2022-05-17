using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Humanizer;
using Humanizer.Localisation;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Listeners;
using ModCore.Logic;
using ModCore.Logic.Extensions;
using ModCore.Logic.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
		public async Task AboutAsync(CommandContext context)
		{
			var eb = new DiscordEmbedBuilder()
				.WithColor(new DiscordColor("#089FDF"))
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
                    $"\n*DSharpPlus:* {context.Client.VersionString}" +
                    $"\n*Servers:* {this.Shared.ModCore.Shards.Select(x => x.Client.Guilds.Count).Sum()}" +
                    $"\n*Shards:* {this.Shared.ModCore.Shards.Count}")
				.AddField("Contribute?", "Contributions are always welcome at our [GitHub repo.](https://github.com/Naamloos/ModCore)")
				.WithThumbnail(context.Client.CurrentUser.AvatarUrl)
				.Build();

			await context.ElevatedRespondAsync(embed: eb);
		}

		[Command("ping"), Description("Check ModCore's API connection status."), CheckDisable]
		public async Task PingAsync(CommandContext context)
		{
			await context.SafeRespondAsync($"🏓 Pong: ({context.Client.Ping}) ms.");
		}

		[Command("prefix"), Description("Check ModCore's current prefix."), CheckDisable]
		public async Task PrefixAsync(CommandContext context)
		{
			await context.IfGuildSettings(
				async (e) => await context.SafeRespondAsync($"ℹ️ Current prefix: {e.Prefix}"),
				async () => await context.SafeRespondAsync($"ℹ️ Current prefix: {this.Shared.DefaultPrefix}"));
		}

		[Command("uptime"), Description("Check ModCore's uptime."), Aliases("u"), CheckDisable]
		public async Task UptimeAsync(CommandContext context)
		{
			var starttimes = this.StartTimes;

			await context.SafeRespondUnformattedAsync(
				$"⏱️ Program start: {string.Format("<t:{0}:R>", starttimes.ProcessStartTime.ToUnixTimeSeconds())}\n" +
				$"⏱️ Socket start: {string.Format("<t:{0}:R>", starttimes.SocketStartTime.ToUnixTimeSeconds())}");
		}

		[Command("invite"), Description("Get an invite to this ModCore instance. Sharing is caring!"), Aliases("inv"), CheckDisable]
		public async Task InviteAsync(CommandContext context)
		{
			//TODO replace with a link to a nice invite builder!
			// what the hell is an invite builder? - chris
			var app = context.Client.CurrentApplication;
			if (app.IsPublic != null && (bool)app.IsPublic)
				await context.SafeRespondAsync(
					$"🛡️ Add ModCore to your server!\n<https://modcore.naamloos.dev/info/invite>");
			else
				await context.SafeRespondUnformattedAsync("⚠️ I'm sorry Mario, but this instance of ModCore has been set to private!");
		}

		[Command("ban"), Description("Bans a member."), Aliases("b"), RequirePermissions(Permissions.BanMembers), CheckDisable]
		public async Task BanAsync(CommandContext ctx, [Description("Member to ban")] DiscordMember member,
			[RemainingText, Description("Reason to ban this member")] string reason = "")
		{
			if (ctx.Member.Id == member.Id)
			{
				await ctx.SafeRespondUnformattedAsync("⚠️ You can't do that to yourself! You have so much to live for!");
				return;
			}

			var userstring = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
			var reasonstring = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
			var sent_dm = false;
			try
			{
				await member.ElevatedMessageAsync($"🚓 You've been banned from {ctx.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the follwing reason:\n```\n{reason}\n```")}");
				sent_dm = true;
			}
			catch (Exception) { }

			await ctx.Guild.BanMemberAsync(member, 7, $"{userstring}{reasonstring}");
			await ctx.SafeRespondAsync($"🚓 Banned user {member.DisplayName} (ID:{member.Id}).\n{(sent_dm ? "Said user has been notified of this action." : "")}");

			await ctx.LogActionAsync($"🚓 Banned user {member.DisplayName} (ID:{member.Id})\n{reasonstring}");
		}

		[Command("nukeban")]
		[Description("Bans a member from all servers you own that have ModCore")]
		public async Task NukeBanAsync(CommandContext context, ulong userId, string reason = "")
		{
			await context.RespondAsync($"‼️ This will ban the user with ID {userId} from all servers you own. Proceed?" +
				$"\n**Be wary that this will ACTUALLY ban them from all servers you own, whether they are part of this server or not.**");
			var response = await context.Message.GetNextMessageAsync();
			if (!response.TimedOut && (response.Result?.Content.ToLower() == "yes" || response.Result?.Content.ToLower() == "y"))
			{
				int skip = 0;
				var servers = this.Shared.ModCore.Shards.SelectMany(x => x.Client.Guilds.Values).Where(x => x.Owner.Id == context.Member.Id);
				foreach(var server in servers)
				{
					if (server.CurrentMember.Roles.Any(x => x.CheckPermission(Permissions.BanMembers) == PermissionLevel.Allowed))
					{
						await server.BanMemberAsync(userId, 0, $"[ModCore NukeBan] {reason}");
					}
					else
					{
						skip++;
					}
				}
				await context.RespondAsync($"🚓 Succesfully nukebanned member from {servers.Count()} servers." +
					$"{(skip > 0? $" Skipped {skip} servers due to lacking permissions" : "")}");
			}
			else
			{
				await context.RespondAsync("Action canceled.");
			}
		}

		[Command("hackban"), Description("Ban an user by their ID. The user does not need to be in the guild."),
		 Aliases("hb"), RequirePermissions(Permissions.BanMembers), CheckDisable]
		public async Task HackBanAsync(CommandContext context, [Description("ID of user to ban")]ulong id,
			[RemainingText, Description("Reason to ban this member")] string reason = "")
		{
			if (context.Member.Id == id)
			{
				await context.SafeRespondUnformattedAsync("⚠️ You can't do that to yourself! You have so much to live for!");
				return;
			}

			var userstring = $"{context.User.Username}#{context.User.Discriminator} ({context.User.Id})";
			var reasonstring = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
			await context.Guild.BanMemberAsync(id, 7, $"{userstring}{reasonstring}");
			await context.SafeRespondUnformattedAsync("🚓 User hackbanned successfully.");

			await context.LogActionAsync($"🚓 Hackbanned ID: {id}\n{reasonstring}");
		}

		[Command("kick"), Description("Kicks a member from the guild. Can optionally provide a reason for kick."),
		 Aliases("k"), RequirePermissions(Permissions.KickMembers), CheckDisable]
		public async Task KickAsync(CommandContext context, [Description("Member to kick")]DiscordMember member,
			[RemainingText, Description("Reason to kick this member")] string reason = "")
		{
			if (context.Member.Id == member.Id)
			{
				await context.SafeRespondUnformattedAsync("⚠️ You can't do that to yourself! You have so much to live for!");
				return;
			}

			var userstring = $"{context.User.Username}#{context.User.Discriminator} ({context.User.Id})";
			var reasonstring = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
			var sent_dm = false;
			try
			{
				await member.ElevatedMessageAsync($"🚓 You've been kicked from {context.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the follwing reason:\n```\n{reason}\n```")}");
				sent_dm = true;
			}
			catch (Exception ex) { }

			await member.RemoveAsync($"{userstring}{reasonstring}");
			await context.SafeRespondAsync($"🚓 Kicked user {member.DisplayName} (ID:{member.Id}).\n{(sent_dm ? "Said user has been notified of this action." : "")}");

			await context.LogActionAsync($"🚓 Kicked user {member.DisplayName} (ID:{member.Id})\n{reasonstring}");
		}

		[Command("softban"),
		 Description("Bans then unbans an user from the guild. " +
					 "This will delete their recent messages, but they can join back."), Aliases("sb"),
		 RequireUserPermissions(Permissions.KickMembers), RequireBotPermissions(Permissions.BanMembers), CheckDisable]
		public async Task SoftbanAsync(CommandContext context, [Description("Member to softban")]DiscordMember member,
			[RemainingText, Description("Reason to softban this member")] string reason = "")
		{
			if (context.Member.Id == member.Id)
			{
				await context.SafeRespondUnformattedAsync("⚠️ You can't do that to yourself! You have so much to live for!");
				return;
			}

			var userstring = $"{context.User.Username}#{context.User.Discriminator} ({context.User.Id})";
			var reasonstring = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
			var sent_dm = false;
			try
			{
				await member.ElevatedMessageAsync($"🚓 You've been kicked from {context.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the follwing reason:\n```\n{reason}\n```")}");
				sent_dm = true;
			}
			catch (Exception) { }

			await member.BanAsync(7, $"{userstring}{reasonstring} (softban)");
			await member.UnbanAsync(context.Guild, $"{userstring}{reasonstring}");
			await context.SafeRespondAsync($"🚓 Softbanned user {member.DisplayName} (ID:{member.Id}).\n{(sent_dm ? "Said user has been notified of this action." : "")}");

			await context.LogActionAsync($"🚓 Softbanned user {member.DisplayName} (ID:{member.Id})\n{reasonstring}");
		}

		[Command("leave"), Description("Makes this bot leave the current server. Goodbye."),
		 RequireUserPermissions(Permissions.Administrator), CheckDisable]
		public async Task LeaveAsync(CommandContext context)
		{
			var interactivity = this.Interactivity;
			await context.SafeRespondUnformattedAsync("❓ Are you sure you want to remove modcore from your guild?");
			var message = await interactivity.WaitForMessageAsync(
				x => x.ChannelId == context.Channel.Id && x.Author.Id == context.Member.Id, TimeSpan.FromSeconds(30));

			if (message.Result == null)
				await context.SafeRespondUnformattedAsync("Timed out.");
			else if (message.Result.Content.ToLowerInvariant() == "yes")
			{
				await context.SafeRespondUnformattedAsync("❤️ Thanks for using ModCore. Leaving this guild.");
				await context.LogActionAsync("Left your server. Thanks for using ModCore.");
				await context.Guild.LeaveAsync();
			}
			else
				await context.SafeRespondUnformattedAsync("Operation canceled by user.");
		}

		[Command("tempban"), Aliases("tb"), Description(
			 "Temporarily bans a member. They will be automatically unbanned " +
			 "after a set amount of time."),
		 RequirePermissions(Permissions.BanMembers), CheckDisable]
		public async Task TempBanAsync(CommandContext context, [Description("Member to ban temporarily")]DiscordMember member,
			[Description("How long this member will be banned")]TimeSpan timespan, [Description("Why this member got banned")]string reason = "")
		{
			if (context.Member.Id == member.Id)
			{
				await context.SafeRespondUnformattedAsync("⚠️ You can't do that to yourself! You have so much to live for!");
				return;
			}

			var unbanmoment = DateTimeOffset.UtcNow.Add(timespan);

			var userstring = $"{context.User.Username}#{context.User.Discriminator} ({context.User.Id})";
			var reasonstring = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
			var sent_dm = false;
			try
			{
				await member.ElevatedMessageAsync($"🚓 You've been temporarily banned from {context.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the following reason:\n```\n{reason}\n```")}" +
					$"\nYou can rejoin <t:{unbanmoment.ToUnixTimeSeconds()}:R>");
				sent_dm = true;
			}
			catch (Exception) { }

			await member.BanAsync(7, $"{userstring}{reasonstring}");
			// Add timer
			var currentTime = DateTimeOffset.UtcNow;
			var dispatchTime = currentTime + timespan;

			var reminder = new DatabaseTimer
			{
				GuildId = (long)context.Guild.Id,
				ChannelId = 0,
				UserId = (long)member.Id,
				DispatchAt = dispatchTime.LocalDateTime,
				ActionType = TimerActionType.Unban
			};

			reminder.SetData(new TimerUnbanData
			{
				Discriminator = member.Discriminator,
				DisplayName = member.Username,
				UserId = (long)member.Id
			});

			using (var db = this.Database.CreateContext())
			{
				db.Timers.Add(reminder);
				await db.SaveChangesAsync();
			}

			await Timers.RescheduleTimers(context.Client, this.Database, this.Shared);

			// End of Timer adding
			await context.SafeRespondAsync(
				$"🚓 Tempbanned user {member.DisplayName} (ID:{member.Id}) to be unbanned in {timespan.Humanize(4, minUnit: TimeUnit.Second)}.\n{(sent_dm ? "Said user has been notified of this action." : "")}");

			await context.LogActionAsync(
				$"🚓 Tempbanned user {member.DisplayName} (ID:{member.Id}) to be unbanned in {timespan.Humanize(4, minUnit: TimeUnit.Second)}");
		}

		[Command("timeout"), Aliases("mute", "tempmute", "tm", "m"), Description("Temporarily mutes a member. They will be automatically " +
														 "unmuted after a set amount of time. This will prevent them " +
														 "from speaking in chat."),
		 RequirePermissions(Permissions.MuteMembers), CheckDisable]
		public async Task TempMuteAsync(CommandContext context, [Description("Member to temporarily mute")]DiscordMember member,
			[Description("How long this member will be muted")]TimeSpan timespan, [Description("Reason to temp mute this member")]string reason = "")
		{
			if (context.Member.Id == member.Id)
			{
				await context.SafeRespondUnformattedAsync("⚠️ No need to mute yourself bruv");
				return;
			}

			var timeoutEnd = DateTimeOffset.UtcNow.Add(timespan);

			var userstring = $"{context.User.Username}#{context.User.Discriminator} ({context.User.Id})";
			var reasonstring = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
			var sent_dm = false;
			try
			{
				await member.ElevatedMessageAsync($"🚓 You've been temporarily muted in {context.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the following reason:\n```\n{reason}\n```")}" +
					$"\nYou can talk again <t:{timeoutEnd.ToUnixTimeSeconds()}:R>");
				sent_dm = true;
			}
			catch (Exception) { }

			await member.TimeoutAsync(timeoutEnd, reasonstring);
			
			// End of Timer adding
			await context.SafeRespondAsync(
				$"🚓 Tempmuted user {member.DisplayName} (ID:{member.Id}) to be unmuted in {timespan.Humanize(4, minUnit: TimeUnit.Second)}.\n{(sent_dm ? "Said user has been notified of this action." : "")}");

			await context.LogActionAsync(
				$"🚓 Tempmuted user {member.DisplayName} (ID:{member.Id}) to be unmuted in {timespan.Humanize(4, minUnit: TimeUnit.Second)}");
		}

		[Command("unmute"), Description("Unmutes an user previously muted with the mute command. Let them speak!"),
		 Aliases("um"), RequirePermissions(Permissions.MuteMembers),
		 RequireBotPermissions(Permissions.ManageRoles), CheckDisable]
		public async Task UnmuteAsync(CommandContext context, [Description("Member to unmute")] DiscordMember message,
			[RemainingText, Description("Reason to unmute this member")] string reason = "")
		{
			if (context.Member.Id == message.Id)
			{
				await context.SafeRespondUnformattedAsync("⚠️ You can't really execute this command if you're muted yourself...");
				return;
			}

			var userstring = $"{context.User.Username}#{context.User.Discriminator} ({context.User.Id})";
			var reasonstring = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
			var sent_dm = false;
			try
			{
				await message.ElevatedMessageAsync($"🚓 You've been unmuted in {context.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the follwing reason:\n```\n{reason}\n```")}");
				sent_dm = true;
			}
			catch (Exception) { }

			await message.TimeoutAsync(null, $"{userstring}{reasonstring} (unmute)");
			await context.SafeRespondAsync(
				$"🚓 Unmuted user {message.DisplayName} (ID:{message.Id}) {(reason != "" ? "With reason: " + reason : "")}.\n{(sent_dm ? "Said user has been notified of this action." : "")}");

			await context.LogActionAsync(
				$"🚓 Unmuted user {message.DisplayName} (ID:{message.Id}) {(reason != "" ? "With reason: " + reason : "")}");
		}

		[Command("schedulepin"), Aliases("sp"), Description("Schedules a pinned message. _I really don't know why " +
															"you'd want to do this._"),
		 RequirePermissions(Permissions.ManageMessages), CheckDisable]
		public async Task SchedulePinAsync(CommandContext context, [Description("Message to schedule a pin for")]DiscordMessage message,
			[Description("How long it will take for this message to get pinned")]TimeSpan timespan)
		{
			// Add timer
			var currentTime = DateTimeOffset.UtcNow;
			var dispatchTime = currentTime + timespan;

			var reminder = new DatabaseTimer
			{
				GuildId = (long)context.Guild.Id,
				ChannelId = (long)context.Channel.Id,
				UserId = (long)context.User.Id,
				DispatchAt = dispatchTime.LocalDateTime,
				ActionType = TimerActionType.Pin
			};

			reminder.SetData(new TimerPinData { MessageId = (long)message.Id, ChannelId = (long)context.Channel.Id });
			using (var db = this.Database.CreateContext())
			{
				db.Timers.Add(reminder);
				await db.SaveChangesAsync();
			}

			await Timers.RescheduleTimers(context.Client, this.Database, this.Shared);

			// End of Timer adding
			await context.SafeRespondAsync(
				$"✅ This message will be pinned <t:{DateTimeOffset.UtcNow.Add(timespan).ToUnixTimeSeconds()}:R>.");
		}

		[Command("scheduleunpin"), Aliases("sup"), Description("Schedules unpinning a pinned message. This command " +
															   "really is useless, isn't it?"),
		 RequirePermissions(Permissions.ManageMessages), CheckDisable]
		public async Task ScheduleUnpinAsync(CommandContext context, [Description("Message to schedule unpinning for")]DiscordMessage message,
			[Description("Time it will take before this message gets unpinned")]TimeSpan timespan)
		{
			if (!message.Pinned)
				await message.PinAsync();
			// Add timer
			var currentTime = DateTimeOffset.UtcNow;
			var dispatchTime = currentTime + timespan;

			var reminder = new DatabaseTimer
			{
				GuildId = (long)context.Guild.Id,
				ChannelId = (long)context.Channel.Id,
				UserId = (long)context.User.Id,
				DispatchAt = dispatchTime.LocalDateTime,
				ActionType = TimerActionType.Unpin
			};

			reminder.SetData(new TimerUnpinData { MessageId = (long)message.Id, ChannelId = (long)context.Channel.Id });
			using (var db = this.Database.CreateContext())
			{
				db.Timers.Add(reminder);
				await db.SaveChangesAsync();
			}

			await Timers.RescheduleTimers(context.Client, this.Database, this.Shared);

			// End of Timer adding
			await context.SafeRespondAsync(
				$"✅ This message will be unpinned <t:{DateTimeOffset.UtcNow.Add(timespan).ToUnixTimeSeconds()}:R>.");
		}

		[Command("listbans"), Aliases("lb"), Description("Lists banned users. Real complex stuff."), RequireUserPermissions(Permissions.ViewAuditLog), CheckDisable]
		[RequireBotPermissions(Permissions.BanMembers)]
        public async Task ListBansAsync(CommandContext context)
		{
			var bans = await context.Guild.GetBansAsync();
			if (bans.Count == 0)
			{
				await context.SafeRespondUnformattedAsync("No user is banned.");
				return;
			}

			var interactivity = this.Interactivity;
			var page = 1;
			var total = bans.Count / 10 + (bans.Count % 10 == 0 ? 0 : 1);
			var pages = new List<Page>();
			var currentEmbed = new DiscordEmbedBuilder
			{
				Title = "🔨 Banned users:",
				Footer = new DiscordEmbedBuilder.EmbedFooter
				{
					Text = $"Page {page} of {total}"
				}
			};

			foreach (var ban in bans)
			{
				var user = ban.User;
				var reason = (string.IsNullOrWhiteSpace(ban.Reason) ? "No reason given." : ban.Reason);
				currentEmbed.AddField(
					$"{user.Username}#{user.Discriminator} (ID: {user.Id})",
					$"{reason}");
				if (currentEmbed.Fields.Count < 10) continue;
				page++;
				pages.Add(new Page("", currentEmbed));
				currentEmbed = new DiscordEmbedBuilder
				{
					Title = "🔨 Banned users",
					Footer = new DiscordEmbedBuilder.EmbedFooter
					{
						Text = $"Page {page} of {total}"
					}
				};
			}

			if (currentEmbed.Fields.Count > 0)
				pages.Add(new Page("", currentEmbed));

			if (pages.Count > 1)
				await interactivity.SendPaginatedMessageAsync(context.Channel, context.User, pages.ToArray(), new PaginationEmojis());
			else
				await context.ElevatedRespondAsync(embed: pages.First().Embed);
		}

		[Command("announce"), Description("Announces a message to a channel, additionally mentioning a role.")]
		[RequireBotPermissions(Permissions.ManageRoles), RequireUserPermissions(Permissions.MentionEveryone), CheckDisable]
		public async Task AnnounceAsync(CommandContext context, [Description("Role to announce for")]DiscordRole role,
			[Description("Channel to announce to")]DiscordChannel channel, [RemainingText, Description("Announcement text")] string message)
		{
			if (!role.IsMentionable)
			{
				await role.ModifyAsync(mentionable: true);
				var discordMessage = await channel.SafeMessageAsync($"{role.Mention} {message}", context);

				if (channel.Type == ChannelType.News)
					await channel.CrosspostMessageAsync(discordMessage);

				await role.ModifyAsync(mentionable: false);
				await context.Message.DeleteAsync();
				await context.LogActionAsync($"✅ Announced {message}\nTo channel: #{channel.Name}\nTo role: {role.Name}");
			}
			else
			{
				await context.Channel.SafeMessageUnformattedAsync("⚠️ You can't announce to that role because it is mentionable!", true);
				await context.LogActionAsync(
					$"⚠️ Failed announcement\nMessage: {message}\nTo channel: #{channel.Name}\nTo role: {role.Name}");
			}
		}

		[Command("poll"), Description("Creates a reaction-based poll."), CheckDisable]
		[RequireBotPermissions(Permissions.ManageMessages)]
		public async Task PollAsync(CommandContext context, [Description("Question to ask")]string message, 
			[Description("Time to run poll")]TimeSpan timespan, [Description("Reaction options")]params DiscordEmoji[] options)
		{
			await context.Message.DeleteAsync();
			var pollmessage = await context.SafeRespondAsync($"**[Poll]**: {message}");
			var intr = context.Client.GetInteractivity();
			var responses = await intr.DoPollAsync(pollmessage, options, PollBehaviour.DeleteEmojis, timespan);
			StringBuilder sb = new StringBuilder($"**[Poll]**: {message}");
			foreach (var emoji in responses)
			{
				sb.AppendLine();
				sb.Append($"{emoji.Emoji.ToString()}: {emoji.Total}");
			}
			await context.SafeModifyUnformattedAsync(pollmessage, sb.ToString());
		}

		[Command("buildmessage"), Description("Builds a message for you, complete with embeds."), CheckDisable]
		[RequirePermissions(Permissions.ManageMessages)]
		public async Task BuildMessageAsync(CommandContext context)
		{
			var message = await context.RespondAsync($"👋 Hello and welcome to the ModCore message builder!\nI am your host Smirky, and I will guide you through the creation of a message! "
				+ DiscordEmoji.FromName(context.Client, ":smirk:").ToString());
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
				await message.ModifyAsync("", menu);
				var response = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == context.Member.Id && x.ChannelId == context.Channel.Id);
				if (response.Result == null)
				{
					await message.ModifyAsync("⚠️⌛ Message building timed out.");
					return;
				}

				await response.Result.DeleteAsync();

				InteractivityResult<DiscordMessage> userInput; 
				switch (response.Result.Content.ToLower())
				{
					default:
						#region Invalid Response
						await message.ModifyAsync("⚠️ Invalid response.");
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "0":
						#region Set Content
						await message.ModifyAsync("❓ What would you like to set the Content to?");
						userInput = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == context.Member.Id && x.ChannelId == context.Channel.Id);
						if (!userInput.TimedOut)
						{
							content = userInput.Result.Content;
							await userInput.Result.DeleteAsync();
							await message.ModifyAsync($"✅ Content set.");
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "1":
						#region Set Title
						await message.ModifyAsync("❓ What would you like to set the Title to?");
						userInput = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == context.Member.Id && x.ChannelId == context.Channel.Id);
						if (!userInput.TimedOut)
						{
							embed.WithTitle(userInput.Result.Content);
							await userInput.Result.DeleteAsync();
							await message.ModifyAsync($"✅ Title set.");
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "2":
						#region Set Description
						await message.ModifyAsync("❓ What would you like to set the Description to?");
						userInput = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == context.Member.Id && x.ChannelId == context.Channel.Id);
						if (!userInput.TimedOut)
						{
							embed.WithDescription(userInput.Result.Content);
							await userInput.Result.DeleteAsync();
							await message.ModifyAsync($"✅ Description set.");
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "3":
						#region Set Image
						await message.ModifyAsync("❓ What would you like to set the Image URL to?");
						userInput = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == context.Member.Id && x.ChannelId == context.Channel.Id);
						if (!userInput.TimedOut)
						{
							embed.WithImageUrl(userInput.Result.Content);
							await userInput.Result.DeleteAsync();
							await message.ModifyAsync($"✅ Image URL set.");
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "4":
						#region Set Thumbnail
						await message.ModifyAsync("❓ What would you like to set the Thumbnail Image URL to?");
						userInput = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == context.Member.Id && x.ChannelId == context.Channel.Id);
						if (!userInput.TimedOut)
						{
							embed.WithThumbnail(userInput.Result.Content);
							await userInput.Result.DeleteAsync();
							await message.ModifyAsync($"✅ Thumbnail Image URL set.");
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "5":
						#region Set Timestamp
						// Do some fancy pancy timestamp parsing
						await message.ModifyAsync("❓ What would you like to set the Timestamp to?");
						userInput = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == context.Member.Id && x.ChannelId == context.Channel.Id);
						if (!userInput.TimedOut)
						{
                            var dtoc = new DateTimeOffsetConverter() as IArgumentConverter<DateTimeOffset>;
							var ts = await dtoc.ConvertAsync(userInput.Result.Content, context);
							if (ts.HasValue)
							{
								embed.WithTimestamp(ts.Value);
								await userInput.Result.DeleteAsync();
								await message.ModifyAsync($"✅ Timestamp set.");
							}
							else
							{
								await userInput.Result.DeleteAsync();
								await message.ModifyAsync($"⚠️ Invalid timestamp.");
							}
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "6":
						#region Set Url
						await message.ModifyAsync("❓ What would you like to set the URL to?");
						userInput = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == context.Member.Id && x.ChannelId == context.Channel.Id);
						if (!userInput.TimedOut)
						{
							embed.WithUrl(userInput.Result.Content);
							await userInput.Result.DeleteAsync();
							await message.ModifyAsync($"✅ URL set.");
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "7":
						#region Set Color
						await message.ModifyAsync("❓ What would you like to set the Color to? (HTML)");
						userInput = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == context.Member.Id && x.ChannelId == context.Channel.Id);
						if (!userInput.TimedOut)
						{
							embed.WithColor(new DiscordColor(userInput.Result.Content));
							await userInput.Result.DeleteAsync();
							await message.ModifyAsync($"✅ Color set.");
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
						#endregion
					case "8":
						#region Set Footer Text
						await message.ModifyAsync("❓ What would you like to set the Footer text to?");
						userInput = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == context.Member.Id && x.ChannelId == context.Channel.Id);
						if (!userInput.TimedOut)
						{
							if (embed.Footer == null)
								embed.WithFooter(userInput.Result.Content);
							else
								embed.Footer.Text = userInput.Result.Content;
							await userInput.Result.DeleteAsync();
							await message.ModifyAsync($"✅ Footer text set.");
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "8b":
						#region Set Footer Icon
						await message.ModifyAsync("❓ What would you like to set the Footer icon URL to?");
						userInput = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == context.Member.Id && x.ChannelId == context.Channel.Id);
						if (!userInput.TimedOut)
						{
							if (embed.Footer == null)
								embed.WithFooter(null, userInput.Result.Content);
							else
								embed.Footer.IconUrl = userInput.Result.Content;

							await userInput.Result.DeleteAsync();
							await message.ModifyAsync($"✅ Footer icon set.");
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "9":
						#region Set Author Name
						await message.ModifyAsync("❓ What would you like to set the Author name to?");
						userInput = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == context.Member.Id && x.ChannelId == context.Channel.Id);
						if (!userInput.TimedOut)
						{
							if (embed.Author == null)
								embed.WithAuthor(userInput.Result.Content);
							else
								embed.Author.Name = userInput.Result.Content;
							await userInput.Result.DeleteAsync();
							await message.ModifyAsync($"✅ Author name set.");
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "9b":
						#region Set Author Icon
						await message.ModifyAsync("❓ What would you like to set the Author Icon URL to?");
						userInput = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == context.Member.Id && x.ChannelId == context.Channel.Id);
						if (!userInput.TimedOut)
						{
							if (embed.Author == null)
								embed.WithAuthor(iconUrl: userInput.Result.Content);
							else
								embed.Author.IconUrl = userInput.Result.Content;
							await userInput.Result.DeleteAsync();
							await message.ModifyAsync($"✅ Author Icon set.");
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "9c":
						#region Set Author Icon
						await message.ModifyAsync("❓ What would you like to set the Author URL to?");
						userInput = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == context.Member.Id && x.ChannelId == context.Channel.Id);
						if (!userInput.TimedOut)
						{
							if (embed.Author == null)
								embed.WithAuthor(url: userInput.Result.Content);
							else
								embed.Author.Url = userInput.Result.Content;
							await userInput.Result.DeleteAsync();
							await message.ModifyAsync($"✅ Author URL set.");
						}
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
					#endregion
					case "10":
						#region Add Field
						(string, string, bool) field;

						await message.ModifyAsync("❓ What should the field title be?");
						userInput = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == context.Member.Id && x.ChannelId == context.Channel.Id);
						if (userInput.TimedOut)
							break;
						field.Item1 = userInput.Result.Content;
						await userInput.Result.DeleteAsync();

						await message.ModifyAsync("❓ What should the field content be?");
						userInput = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == context.Member.Id && x.ChannelId == context.Channel.Id);
						if (userInput.TimedOut)
							break;
						field.Item2 = userInput.Result.Content;
						await userInput.Result.DeleteAsync();

						await message.ModifyAsync("❓ Should the field be inline?");
						userInput = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == context.Member.Id && x.ChannelId == context.Channel.Id);
						if (userInput.TimedOut)
							break;
						var bcv = new AugmentedBoolConverter();
						var inl = await bcv.ConvertAsync(userInput.Result.Content, context);
						field.Item3 = (inl.HasValue? inl.Value : false);
						await userInput.Result.DeleteAsync();

						embed.AddField(field.Item1, field.Item2, field.Item3);
						await message.ModifyAsync("✅ Field added.");
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
						#endregion
					case "11":
						#region Clear Fields
						embed.ClearFields();
						await message.ModifyAsync("✅ Cleared fields.");
						await Task.Delay(TimeSpan.FromSeconds(2));
						break;
						#endregion
					case "12":
						#region Send Message
						// Remember to pick a channel to send to first!!
						await message.ModifyAsync("What channel do you want to send this message to?");
						userInput = await this.Interactivity.WaitForMessageAsync(x => x.Author.Id == context.Member.Id && x.ChannelId == context.Channel.Id);
						if (!userInput.TimedOut)
						{
							var dcc = new DiscordChannelConverter() as IArgumentConverter<DiscordChannel>;
							var channel = await dcc.ConvertAsync(userInput.Result.Content, context);
							if (channel.HasValue)
							{
								await channel.Value.SendMessageAsync(content, embed: embed == new DiscordEmbedBuilder()? embed.Build() : null);
								await userInput.Result.DeleteAsync();
								await message.ModifyAsync("✅ Message sent.");
								return;
							}
							else
							{
								await userInput.Result.DeleteAsync();
								await message.ModifyAsync("⚠️ Invalid channel.");
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
						await message.ModifyAsync(content, preview);
						await Task.Delay(TimeSpan.FromSeconds(3));
						break;
					#endregion
					case "14":
						#region Cancel Building
						await message.ModifyAsync("✅ Message building canceled.");
						building = false;
						break;
						#endregion
				}
			}
		}

		[Command("distance")]
		[Description("Counts the amount of messages until a specific Message")]
		public async Task DistanceAsync(CommandContext context, DiscordMessage message)
		{
			if(DateTimeOffset.Now.Subtract(message.Timestamp).TotalDays > 1)
			{
				await context.RespondAsync("⚠️ Yeah.. Can't do that for messages older than a day");
				return;
			}

			var ms = new List<ulong>();
			while (!ms.Contains(context.Message.Id))
			{
				var m = await context.Channel.GetMessagesAfterAsync(message.Id, 100);
				foreach(var mm in m)
				{
					if (!ms.Contains(mm.Id))
						ms.Add(mm.Id);
				}
			}

			await context.RespondAsync($"📃 Counted {ms.Count} Messages.");
		}

		[Command("nick")]
		#if !DEBUG
		[Cooldown(1, 60, CooldownBucketType.User)]
		#endif
		[Description("Requests a nickname change to the server staff")]
		public async Task RequestNicknameChangeAsync(CommandContext context, [RemainingText] string nickname)
		{
			if (nickname == context.Member.Nickname)
			{
				await context.ElevatedRespondAsync("⚠️ That's already your nickname.");
				return;
			}
			if (nickname == context.Member.Username)
			{
				await context.ElevatedRespondAsync("⚠️ That's already your username.");
				return;
			}
			
			var yes = DiscordEmoji.FromName(context.Client, ":white_check_mark:");
			var no = DiscordEmoji.FromName(context.Client, ":negative_squared_cross_mark:");

			// attempt to automatically change the person's nickname if they can already change it on their own,
			// and prompt them if we're not able to
			if (context.Member.PermissionsIn(context.Channel).HasPermission(Permissions.ChangeNickname))
			{
				if (context.Guild.CurrentMember.PermissionsIn(context.Channel).HasPermission(Permissions.ManageNicknames) && 
				    context.Guild.CurrentMember.CanInteract(context.Member))
				{
					await context.Member.ModifyAsync(member =>
					{
						member.Nickname = nickname;
						member.AuditLogReason = "Nickname change requested by " +
						                        "@{reaction.User.Username}#{reaction.User.Discriminator} auto approved " +
						                        "since they already have the Change Nickname permission";
					});
					await context.Message.CreateReactionAsync(yes);
					return;
				}

				await context.ElevatedRespondAsync("⚠️ Do it yourself, you have the permission!");
				return;
			}

			await context.WithGuildSettings(async config =>
			{
				// don't change the member's nickname here, as that violates the hierarchy of permissions
				if (!config.RequireNicknameChangeConfirmation)
				{
					if (context.Member == context.Guild.Owner)
					{
						await context.ElevatedRespondAsync("⚠️ Use the `config nickchange enable` command to enable nickname " +
						                               "change requests.");
					}
					else
					{
						await context.ElevatedRespondAsync("⚠️ The server owner has disabled nickname changing on this server.");
					}

					return;
				}
				
				// only say it's unable to process if BOTH the confirmation requirement is enabled and the bot doesn't
				// have the permissions for it
				if (!context.Guild.CurrentMember.PermissionsIn(context.Channel).HasPermission(Permissions.ManageNicknames) ||
				    !context.Guild.CurrentMember.CanInteract(context.Member))
				{
					await context.ElevatedRespondAsync("⚠️ Unable to process nickname change because the bot lacks the " +
					                               "required permissions, or cannot action on this member.");
					return;
				}
				
				var message = await context.Guild.GetChannel(config.NicknameChangeConfirmationChannel)
					.SendMessageAsync(embed: new DiscordEmbedBuilder()
                    .WithTitle("Nickname change confirmation")
                    .WithDescription($"Member {context.Member.Mention} ({context.Member.Id}) wants to change their nickname to " +
                            $"{Formatter.Sanitize(nickname)}.")
                    .WithFooter("This message will self-destruct in 2 hours."));

				// d#+ nightlies mean we can do this now, and hopefully it won't crash
				await Task.WhenAll(message.CreateReactionAsync(yes), message.CreateReactionAsync(no));

				await context.ElevatedRespondAsync(
					"✅ Your request to change username was placed, and should be actioned shortly.");
				
                // TODO: y'all gotta mess with dem timeout shit, else we'll get sum nasty errors
				var result = await this.Interactivity.WaitForReactionAsync(
					e => (e.Emoji == yes || e.Emoji == no) && e.Message == message, timeoutoverride: TimeSpan.FromHours(2));
                var reaction = result.Result;

				if (reaction.Emoji == yes)
				{
					await context.Member.ModifyAsync(member => member.Nickname = nickname);

					await message.DeleteAsync(
						$"✅ Request to change username accepted by @{reaction.User.Username}#{reaction.User.Discriminator}");
					
					await context.Member.SendMessageAsync($"Your name in {context.Guild.Name} was successfully changed to " +
					                                  $"{Formatter.Sanitize(nickname)}.");
					
					try
					{
						await context.Message.CreateReactionAsync(yes);
					} catch { /* empty, message has been deleted */ }
				}
				else
				{
					await message.DeleteAsync(
						$"❌ Request to change username denied by @{reaction.User.Username}#{reaction.User.Discriminator}");
					
					await context.Member.SendMessageAsync(
						$"❌ Your request to change your username in {context.Guild.Name} was denied.");
					
					try
					{
						await context.Message.CreateReactionAsync(no);
					} catch { /* empty, message has been deleted */ }
					
				}

			});
		}

		// TODO: use database timer system??
		// TODO: multiple winners
		[Command("giveaway")]
		[Description("Creates a giveaway")]
		[RequireUserPermissions(Permissions.ManageGuild)]
		[RequireBotPermissions(Permissions.ManageMessages | Permissions.AddReactions)]
		[Cooldown(1, 600, CooldownBucketType.Channel)]
		public async Task GiveawayAsync(CommandContext context, string prize, TimeSpan timespan)
		{
			await context.Message.DeleteAsync();
			var trophy = DiscordEmoji.FromName(context.Client, ":trophy:");
			var giveaway = await context.RespondAsync($"Hey! {context.Member.Mention} is giving away {prize}!\nReact with {trophy.ToString()} to join in!");
			await giveaway.CreateReactionAsync(trophy);

			await Task.Delay(timespan);

			var members = (await giveaway.GetReactionsAsync(trophy)).ToList();
			members.RemoveAll(x => x.Id == context.Client.CurrentUser.Id);

			var winnerindex = new Random().Next(0, members.Count() - 1);
			var winner = members[winnerindex];

			var tada = DiscordEmoji.FromName(context.Client, ":tada:");
			await giveaway.ModifyAsync($"{tada.ToString()}{tada.ToString()} " +
				$"{winner.Mention}, you won! Contact {context.Member.Mention} for your price! " +
				$"{trophy.ToString()}{trophy.ToString()}");
		}

        [Command("snipe")]
        [Description("Snipes last deleted message")]
        public async Task SnipeAsync(CommandContext context)
        {
            if (this.Shared.DeletedMessages.ContainsKey(context.Channel.Id))
            {
                var message = this.Shared.DeletedMessages[context.Channel.Id];

                var content = message.Content;
                if (content.Length > 500)
                    content = content.Substring(0, 500) + "...";

                var embed = new DiscordEmbedBuilder().WithAuthor($"{message.Author.Username}#{message.Author.Discriminator}", iconUrl: message.Author.GetAvatarUrl(ImageFormat.Png));

				if (!string.IsNullOrEmpty(message.Content))
				{
					embed.WithDescription(message.Content);
					embed.WithTimestamp(message.Id);
				}

				if (message.Attachments.Count > 0)
				{
					if (message.Attachments[0].MediaType == "image/png"
						|| message.Attachments[0].MediaType == "image/jpeg"
						|| message.Attachments[0].MediaType == "image/gif"
						|| message.Attachments[0].MediaType == "image/apng"
						|| message.Attachments[0].MediaType == "image/webp")
						embed.WithImageUrl(message.Attachments[0].Url);
				}

				await context.RespondAsync(embed: embed);
                return;
            }
            await context.RespondAsync("⚠️ No message to snipe!");
        }

        [Command("snipeedit")]
        [Description("Snipes last edited message")]
        public async Task SnipeEditAsync(CommandContext context)
        {
            if (this.Shared.EditedMessages.ContainsKey(context.Channel.Id))
            {
                var message = this.Shared.EditedMessages[context.Channel.Id];

                var content = message.Content;
                if (content.Length > 500)
                    content = content.Substring(0, 500) + "...";

                var embed = new DiscordEmbedBuilder().WithAuthor($"{message.Author.Username}#{message.Author.Discriminator}", iconUrl: message.Author.GetAvatarUrl(ImageFormat.Png));

				if (!string.IsNullOrEmpty(message.Content))
				{
					embed.WithDescription(message.Content);
					embed.WithTimestamp(message.Id);
				}

				if(message.Attachments.Count > 0)
                {
					if(message.Attachments[0].MediaType == "image/png"
						|| message.Attachments[0].MediaType == "image/jpeg"
						|| message.Attachments[0].MediaType == "image/gif"
						|| message.Attachments[0].MediaType == "image/apng"
						|| message.Attachments[0].MediaType == "image/webp")
					embed.WithImageUrl(message.Attachments[0].Url);
                }

                await context.RespondAsync(embed: embed);
                return;
            }
            await context.RespondAsync("⚠️ No message to snipe!");
        }

        [Command("snoop"), Hidden]
        public async Task SnoopAsync(CommandContext ctx)
            => await ctx.RespondAsync("🍃🔥🚬 https://media1.tenor.com/images/48ab2af082ad3d41aa34646e4c467fc1/tenor.gif");

        [Command("cooldown")]
        [Description("Sets a custom message cooldown")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task CooldownAsync(CommandContext context, int cooldown)
        {
            if (cooldown <= 21600 && cooldown >= 0)
            {
                await context.Channel.ModifyAsync(x => x.PerUserRateLimit = cooldown);
                await context.RespondAsync($"✅ Set cooldown to {cooldown} seconds.");
                return;
            }
            await context.RespondAsync($"⚠️ Invalid cooldown: {cooldown}");
        }

        [Command("yoink")]
        [Description("Copies an emoji from a different server to this one")]
        [RequirePermissions(Permissions.ManageEmojis)]
		public async Task YoinkAsync(CommandContext ctx, DiscordEmoji emoji, [RemainingText]string name = "")
        {
			if(!emoji.ToString().StartsWith('<'))
            {
				await ctx.RespondAsync("⚠️ This is not a valid guild emoji!");
				return;
            }				
			await stealieEmoji(ctx, string.IsNullOrEmpty(name)? emoji.Name : name, emoji.Id, emoji.IsAnimated);
		}

		const string EMOJI_REGEX = @"<a?:(.+?):(\d+)>";
        [Command("yoink")]
		[RequirePermissions(Permissions.ManageEmojis)]
		public async Task YoinkAsync(CommandContext ctx, int index = 1)
        {
			if(ctx.Message.ReferencedMessage != null)
            {
				var matches = Regex.Matches(ctx.Message.ReferencedMessage.Content, EMOJI_REGEX);
				if(matches.Count < index || index < 1)
                {
					await ctx.RespondAsync("⚠️ Referenced emoji not found!");
					return;
                }

				var split = matches[index-1].Groups[2].Value;
				var emojiName = matches[index-1].Groups[1].Value;
				var animated = matches[index-1].Value.StartsWith("<a");

				if (ulong.TryParse(split, out ulong emoji_id))
                {
					await stealieEmoji(ctx, emojiName, emoji_id, animated);
					return;
                }
                else
                {
					await ctx.RespondAsync("⚠️ Failed to fetch your new emoji.");
					return;
				}
            }
			await ctx.RespondAsync("⚠️ You need to reply to an existing message to use this command!");
		}

		[Command("yeet")]
		[Description("Deletes an emoji from this server.")]
		[RequirePermissions(Permissions.ManageEmojis)]
		public async Task YeetAsync(CommandContext ctx, DiscordEmoji emoji)
		{
			try
			{
				var guildEmoji = await ctx.Guild.GetEmojiAsync(emoji.Id);
				await ctx.Guild.DeleteEmojiAsync(guildEmoji);
				await ctx.RespondAsync($"⚠️ Deleted emoji {emoji.Name}!");
			}
			catch (Exception) 
			{
				await ctx.RespondAsync("⚠️ This emoji does not belong to this server!");
			} 
		}

        [Command("generatepassword")]
        [Description("Generates a password for you!")]
        [Hidden]
		public async Task GeneratePasswordAsync(CommandContext ctx)
        {
			await ctx.RespondAsync("🤨");
        }

        [Command("offtopic")]
        [Description("Moves off-topic chat to an appropriate channel.")]
		public async Task OffTopicAsync(CommandContext ctx, DiscordChannel channel, params DiscordMember[] members)
        {
			IEnumerable<DiscordMessage> messages = await ctx.Channel.GetMessagesAsync(100);

			string offtopic = $"❗ Your current conversation is off-topic! Most recent messages have been copied to {channel.Mention}. ";

			if (members.Any())
			{
				messages = messages.Where(x => members.Any(y => y.Id == x.Author.Id));

				offtopic += string.Join(" ", members.Select(x => x.DisplayName));
			}
			messages = messages.Take(20).Reverse();

			await ctx.Channel.SendMessageAsync(offtopic);
			await channel.SendMessageAsync($"⚠️ Copying off-topic messages from {ctx.Channel.Mention}!");
			var webhook = await channel.CreateWebhookAsync($"offtopic-move-{new Random().Next()}");
			foreach (var message in messages)
            {
				if (string.IsNullOrEmpty(message.Content))
					continue;

				var webhookMessage = new DiscordWebhookBuilder()
					.WithContent(message.Content)
					.WithAvatarUrl(message.Author.GetAvatarUrl(ImageFormat.Auto))
					.WithUsername((message.Author as DiscordMember).DisplayName);

				await webhook.ExecuteAsync(webhookMessage);
            }
			await webhook.DeleteAsync();
			await channel.SendMessageAsync($"⚠❗ Off topic chat has been copied from {ctx.Channel.Mention}! Please continue conversation here.");
		}

		private async Task stealieEmoji(CommandContext ctx, string name, ulong id, bool animated)
        {
			using HttpClient _client = new HttpClient();
			var downloadedEmoji = await _client.GetStreamAsync($"https://cdn.discordapp.com/emojis/{id}.{(animated ? "gif" : "png")}");
			using MemoryStream memory = new MemoryStream();
			downloadedEmoji.CopyTo(memory);
			downloadedEmoji.Dispose();
			var newEmoji = await ctx.Guild.CreateEmojiAsync(name, memory);
			await ctx.RespondAsync($"✅ Yoink! This emoji has been added to your server: {newEmoji.ToString()}");
		}
    }
}
