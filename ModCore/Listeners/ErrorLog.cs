using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using F23.StringSimilarity;
using Humanizer;
using ModCore.Entities;
using ModCore.Logic;
using System.Text;
using ModCore.Logic.Extensions;
using Microsoft.Extensions.Logging;

namespace ModCore.Listeners
{
	public class ErrorLog
	{
		[AsyncListener(EventTypes.CommandErrored)]
		public static async Task CommandError(ModCoreShard bot, CommandErrorEventArgs eventargs)
		{
			var config = eventargs.Context.GetGuildSettings() ?? new GuildSettings();
			var context = eventargs.Context;

			if (await NotifyCommandDisabled(eventargs, config, context)) return;

			if (await NotifyCommandNotFound(bot, eventargs, config, context)) return;

			await NotifyCommandErrored(eventargs, config, context);
		}

		private static async Task NotifyCommandErrored(CommandErrorEventArgs eventargs, GuildSettings config, CommandContext context)
		{
			var qualifiedName = eventargs.Command.QualifiedName;
			switch (config.CommandError.Chat)
			{
				default:
				case CommandErrorVerbosity.None:
					break;

				case CommandErrorVerbosity.Name:
					await context.SafeRespondAsync($"⚠️ **Command {qualifiedName} Errored!**");
					break;
				case CommandErrorVerbosity.NameDesc:
					await DescribeCommandErrorAsync(eventargs.Exception, qualifiedName, context);
					break;
				case CommandErrorVerbosity.Exception:
					var stream = new MemoryStream();
					var writer = new StreamWriter(stream);
					writer.Write(eventargs.Exception.StackTrace);
					writer.Flush();
					stream.Position = 0;
					await context.RespondAsync(x =>
						x.WithFile("exception.txt", stream)
						.WithContent($"**Command `{qualifiedName}` Errored!**\n`{eventargs.Exception.GetType()}`:\n{eventargs.Exception.Message}"));
					break;
			}

			#if DEBUG
				Console.WriteLine($"Oopsie woopsie! {eventargs.Exception}");
			#endif
		}

		private static async Task<bool> NotifyCommandNotFound(ModCoreShard bot, CommandErrorEventArgs eventargs,
			GuildSettings config, CommandContext context)
		{
			if (eventargs.Exception is not CommandNotFoundException commandNotFound) return false;

			// return instead of proceeding, since qualifiedName below this will throw
			// (since there is no Command obj) 
			if (!config.SpellingHelperEnabled) return true;

			// TODO: this only gives the first invalid token, so "a b" will just recognize as "a".
			//       it should probably take into consideration all the tokens.
			var attemptedName = commandNotFound.CommandName;
			try
			{
				// TODO: i intended on using the library for more than just this,
				//       but i ended up using it like this, so can probably just copy code from that lib 
				//       instead of nugetting it
				var leveshtein = new Levenshtein(); // lower is better

				// TODO: add checks
				var everything = bot.SharedData.Commands.Select(x => x.cmd).ToList();
				var some_of_them = new List<Command>();

				for(int i = 0; i < everything.Count; i++)
				{
					if (await CanExecute(everything[i], context))
						some_of_them.Add(everything[i]);
				}

				var ordered = some_of_them
					.Where(c => c is not CommandGroup group || @group.IsExecutableWithoutSubcommands)
					.Select(c => (qualifiedName: c.QualifiedName, description: c.Description))
					.OrderBy(c => leveshtein.Distance(attemptedName, c.qualifiedName))
					.Take(1).ToArray();

                var embed = new DiscordEmbedBuilder()
                    .WithTitle("⚠️ Command **" + attemptedName.Truncate(200) + "** not found!")
                    .WithDescription("Did you mean...");

                foreach(var (qualifiedName, description) in ordered)
                {
                    embed.AddField(qualifiedName.Truncate(256), description?.Truncate(999) ?? "<no desc>");
                }

                await context.RespondAsync(embed: embed);
			}
			catch (Exception ex)
			{
				eventargs.Context.Client.Logger.Log(LogLevel.Critical, "CommandErrored", ex.ToString(),
					DateTime.Now);
			}

			return true;

		}

		private static async Task<bool> CanExecute(Command command, CommandContext context)
		{
			if (command.Parent != null)
				return await CanExecute(command.Parent, context).ConfigureAwait(false);

			var fchecks = await command.RunChecksAsync(context, true).ConfigureAwait(false);
			if (fchecks.Any())
				return false;
			return true;
		}

		private static async Task<bool> NotifyCommandDisabled(CommandErrorEventArgs eventargs, GuildSettings config,
			CommandContext ctx)
		{
			if (eventargs.Exception is not ChecksFailedException checksFailed ||
				!checksFailed.FailedChecks.Any(x => x is CheckDisableAttribute)) return false;

			if (config.NotifyDisabledCommand)
			{
				await ctx.ElevatedRespondAsync("⚠️ Sorry! that command has been disabled in this guild.");
			}

			return true;
		}

		public static async Task DescribeCommandErrorAsync(Exception ex, string command, CommandContext context)
		{
			if (ex is ChecksFailedException checksFailed)
			{
				var reasons = new List<string>();

				var failed = checksFailed.FailedChecks;

				if (failed.Any(x => x is RequireUserPermissionsAttribute))
					reasons.Add("you don't have the right permissions to execute this command");
				if (failed.Any(x => x is RequireRolesAttribute))
					reasons.Add("you don't have the right roles to execute this command");
				if (failed.Any(x => x is RequirePrefixesAttribute))
					reasons.Add("you can't execute this command with that prefix");
				if (failed.Any(x => x is RequirePermissionsAttribute))
					reasons.Add("one of us doesn't have the right permissions to execute this command");
				if (failed.Any(x => x is RequireOwnerAttribute))
					reasons.Add("you don't own this bot");
				if (failed.Any(x => x is RequireNsfwAttribute))
					reasons.Add("this command can only be executed in NSFW channels");
				if (failed.Any(x => x is RequireBotPermissionsAttribute))
					reasons.Add("I don't have the right permissions to execute this command");

				var response = $"⚠️ I couldn't execute `{command}` because ";
				response += string.Join(" _and_ ", reasons);
				response += "!";
				await context.SafeRespondUnformattedAsync(response);
				return;
			}
			await context.SafeRespondAsync($"⚠️ Command {command} Errored!\n{ex.Message}");
		}
	}
}