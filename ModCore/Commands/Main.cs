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

  //      [Command("snoop"), Hidden]
  //      public async Task SnoopAsync(CommandContext ctx)
  //          => await ctx.RespondAsync("🍃🔥🚬 https://media1.tenor.com/images/48ab2af082ad3d41aa34646e4c467fc1/tenor.gif");

  //      [Command("generatepassword")]
  //      [Description("Generates a password for you!")]
  //      [Hidden]
		//public async Task GeneratePasswordAsync(CommandContext ctx)
  //      {
		//	await ctx.RespondAsync("🤨");
  //      }
    }
}
