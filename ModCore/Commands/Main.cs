using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Humanizer;
using ModCore.Database;
using ModCore.Database.DatabaseEntities;
using ModCore.Database.JsonEntities;
using ModCore.Entities;
using ModCore.Listeners;
using ModCore.Utils;
using ModCore.Utils.Extensions;
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

		[Command("ban"), Description("Bans a member."), Aliases("b"), RequirePermissions(Permissions.BanMembers)]
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

			var embed = new DiscordEmbedBuilder()
								.WithTitle($"Banned Member")
								.AddField("Member", $"{member.DisplayName} ({member.Id}, <@{member.Id}>)")
								.AddField("Reason", $"{reasonstring}")
								.AddField("Responsible Moderator", $"<@{ctx.Member.Id}>")
								.WithColor(DiscordColor.Red);
			await ctx.Guild.ModLogAsync(Database.CreateContext(), embed);
		}

        [Command("massban")]
        [Description("mass bans a group of users- either by ID or mention")]
        [RequirePermissions(Permissions.BanMembers)]
		public async Task MassBanAsync(CommandContext ctx, params ulong[] users)
        {
			List<ulong> failed = new List<ulong>();
			for(int i = 0; i < users.Length; i++)
            {
				try
				{
					await ctx.Guild.BanMemberAsync(users[i]);
				}
				catch(Exception)
                {
					failed.Add(users[i]);
                }
            }

			await ctx.RespondAsync($"Banned users." + (failed.Count > 0? $" Failed to ban: {string.Join(" ", failed.Select(x => $" <@{x}> "))}" : ""));
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
		 Aliases("hb"), RequirePermissions(Permissions.BanMembers)]
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

			var embed = new DiscordEmbedBuilder()
								.WithTitle($"Hackbanned Member")
								.AddField("Member", $"{id}, <@{id}>")
								.AddField("Reason", $"{reasonstring}")
								.AddField("Responsible Moderator", $"<@{context.Member.Id}>")
								.WithColor(DiscordColor.Red);
			await context.Guild.ModLogAsync(Database.CreateContext(), embed);
		}

		[Command("softban"),
		 Description("Bans then unbans an user from the guild. " +
					 "This will delete their recent messages, but they can join back."), Aliases("sb"),
		 RequireUserPermissions(Permissions.KickMembers), RequireBotPermissions(Permissions.BanMembers)]
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

			var embed = new DiscordEmbedBuilder()
				.WithTitle($"Softbanned Member")
				.AddField("Member", $"{member.DisplayName} ({member.Id}, <@{member.Id}>)")
				.AddField("Reason", $"{reasonstring}")
				.AddField("Responsible Moderator", $"<@{context.Member.Id}>")
				.WithColor(DiscordColor.Red);
			await context.Guild.ModLogAsync(Database.CreateContext(), embed);
		}

		[Command("timeout"), Aliases("mute", "tempmute", "tm", "m"), Description("Temporarily mutes a member. They will be automatically " +
														 "unmuted after a set amount of time. This will prevent them " +
														 "from speaking in chat."),
		 RequirePermissions(Permissions.MuteMembers)]
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
				$"🚓 Tempmuted user {member.DisplayName} (ID:{member.Id}) to be unmuted <t:{timeoutEnd.ToUnixTimeSeconds()}:R>.\n{(sent_dm ? "Said user has been notified of this action." : "")}");

			var embed = new DiscordEmbedBuilder()
				.WithTitle($"Timed out Member")
				.AddField("Member", $"{member.Id}, <@{member.Id}>")
				.AddField("Reason", $"{reasonstring}")
				.AddField("Unban Date/Time", $"<t:{timeoutEnd.ToUnixTimeSeconds()}:R>")
				.AddField("Responsible Moderator", $"<@{context.Member.Id}>")
				.WithColor(DiscordColor.Red);
			await context.Guild.ModLogAsync(Database.CreateContext(), embed);
		}

		[Command("unmute"), Description("Unmutes an user previously muted with the mute command. Let them speak!"),
		 Aliases("um"), RequirePermissions(Permissions.MuteMembers),
		 RequireBotPermissions(Permissions.ManageRoles)]
		public async Task UnmuteAsync(CommandContext context, [Description("Member to unmute")] DiscordMember member,
			[RemainingText, Description("Reason to unmute this member")] string reason = "")
		{
			if (context.Member.Id == member.Id)
			{
				await context.SafeRespondUnformattedAsync("⚠️ You can't really execute this command if you're muted yourself...");
				return;
			}

			var userstring = $"{context.User.Username}#{context.User.Discriminator} ({context.User.Id})";
			var reasonstring = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
			var sent_dm = false;
			try
			{
				await member.ElevatedMessageAsync($"🚓 You've been unmuted in {context.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the follwing reason:\n```\n{reason}\n```")}");
				sent_dm = true;
			}
			catch (Exception) { }

			await member.TimeoutAsync(null, $"{userstring}{reasonstring} (unmute)");
			await context.SafeRespondAsync(
				$"🚓 Unmuted user {member.DisplayName} (ID:{member.Id}) {(reason != "" ? "With reason: " + reason : "")}.\n{(sent_dm ? "Said user has been notified of this action." : "")}");

			var embed = new DiscordEmbedBuilder()
				.WithTitle($"Pre-emptively unmuted Member")
				.AddField("Member", $"{member.Id}, <@{member.Id}>")
				.AddField("Reason", $"{reasonstring}")
				.AddField("Responsible Moderator", $"<@{context.Member.Id}>")
				.WithColor(DiscordColor.Red);
			await context.Guild.ModLogAsync(Database.CreateContext(), embed);
		}

		[Command("schedulepin"), Aliases("sp"), Description("Schedules a pinned message. _I really don't know why " +
															"you'd want to do this._"),
		 RequirePermissions(Permissions.ManageMessages)]
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
		 RequirePermissions(Permissions.ManageMessages)]
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

		[Command("poll"), Description("Creates a reaction-based poll.")]
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
        [Aliases("raffle")]
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

        [Command("generatepassword")]
        [Description("Generates a password for you!")]
        [Hidden]
		public async Task GeneratePasswordAsync(CommandContext ctx)
        {
			await ctx.RespondAsync("🤨");
        }
    }
}
