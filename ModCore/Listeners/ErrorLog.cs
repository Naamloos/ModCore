﻿using System;
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
		public static async Task CommandError(ModCoreShard bot, CommandErrorEventArgs e)
		{
			if(e.Exception.GetType() == typeof(ArgumentException))
            {
				await e.Context.ShowHelpForAsync(e.Command.QualifiedName);
				return;
            }
			var cfg = e.Context.GetGuildSettings() ?? new GuildSettings();
			var ctx = e.Context;

			if (await NotifyCommandDisabled(e, cfg, ctx)) return;

			if (await NotifyCommandNotFound(bot, e, cfg, ctx)) return;

			await NotifyCommandErrored(e, cfg, ctx);
		}

		private static async Task NotifyCommandErrored(CommandErrorEventArgs e, GuildSettings cfg, CommandContext ctx)
		{
			var qualifiedName = e.Command.QualifiedName;
			switch (cfg.CommandError.Chat)
			{
				default:
				case CommandErrorVerbosity.None:
#if DEBUG
					Console.WriteLine($"Oopsie woopsie! {e.Exception}");
#endif
					break;

				case CommandErrorVerbosity.Name:
					await ctx.SafeRespondAsync($"**Command {qualifiedName} Errored!**");
#if DEBUG
					Console.WriteLine($"Oopsie woopsie! {e.Exception}");
#endif
					break;
				case CommandErrorVerbosity.NameDesc:
					await DescribeCommandErrorAsync(e.Exception, qualifiedName, ctx);
					break;
				case CommandErrorVerbosity.Exception:
					var stream = new MemoryStream();
					var writer = new StreamWriter(stream);
					writer.Write(e.Exception.StackTrace);
					writer.Flush();
					stream.Position = 0;
					await ctx.RespondWithFileAsync("exception.txt", stream,
						$"**Command `{qualifiedName}` Errored!**\n`{e.Exception.GetType()}`:\n{e.Exception.Message}");
					break;
			}

			if (cfg.ActionLog.Enable)
			{
				switch (cfg.CommandError.ActionLog)
				{
					default:
					case CommandErrorVerbosity.None:
						break;

					case CommandErrorVerbosity.Name:
						await ctx.LogMessageAsync($"**Command {qualifiedName} errored!**\n`{e.Exception.GetType()}`");
						break;
					case CommandErrorVerbosity.NameDesc:
						await ctx.LogMessageAsync(
							$"**Command {qualifiedName} errored!**\n`{e.Exception.GetType()}`:\n{e.Exception.Message}");
						break;
					case CommandErrorVerbosity.Exception:
						var st = e.Exception.StackTrace;

						st = st.Length > 1000 ? st.Substring(0, 1000) : st;
						var b = new DiscordEmbedBuilder().WithDescription(st);
						await ctx.LogMessageAsync(
							$"**Command {qualifiedName} {e.Command.Overloads.First().Arguments} errored!**\n`{e.Exception.GetType()}`:\n{e.Exception.Message}",
							b);
						break;
				}
			}
		}

		private static async Task<bool> NotifyCommandNotFound(ModCoreShard bot, CommandErrorEventArgs e,
			GuildSettings cfg, CommandContext ctx)
		{
			if (!(e.Exception is CommandNotFoundException commandNotFound)) return false;

			// return instead of proceeding, since qualifiedName below this will throw
			// (since there is no Command obj) 
			if (!cfg.SpellingHelperEnabled) return true;

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

				for(int i = 0; i < everything.Count(); i++)
				{
					if (await CanExecute(everything[i], ctx))
						some_of_them.Add(everything[i]);
				}

				var ordered = some_of_them
					.Where(c => !(c is CommandGroup group) || @group.IsExecutableWithoutSubcommands)
					.Select(c => (qualifiedName: c.QualifiedName, description: c.Description))
					.OrderBy(c => leveshtein.Distance(attemptedName, c.qualifiedName))
					.DistinctBy(c => c.qualifiedName).Take(1).ToArray();

                var embed = new DiscordEmbedBuilder()
                    .WithTitle("Command **" + attemptedName.Truncate(200) + "** not found")
                    .WithDescription("Did you mean...");

                foreach(var (qualifiedName, description) in ordered)
                {
                    embed.AddField(qualifiedName.Truncate(256), description?.Truncate(999) ?? "<no desc>");
                }

                await ctx.RespondAsync(embed: embed);
			}
			catch (Exception ex)
			{
				e.Context.Client.Logger.Log(LogLevel.Critical, "CommandErrored", ex.ToString(),
					DateTime.Now);
			}

			return true;

		}

		private static async Task<bool> CanExecute(Command cmd, CommandContext ctx)
		{
			if (cmd.Parent != null)
				return await CanExecute(cmd.Parent, ctx).ConfigureAwait(false);

			var fchecks = await cmd.RunChecksAsync(ctx, true).ConfigureAwait(false);
			if (fchecks.Any())
				return false;
			return true;
		}

		private static async Task<bool> NotifyCommandDisabled(CommandErrorEventArgs e, GuildSettings cfg,
			CommandContext ctx)
		{
			if (!(e.Exception is ChecksFailedException checksFailed) ||
				!checksFailed.FailedChecks.Any(x => x is CheckDisableAttribute)) return false;

			if (cfg.NotifyDisabledCommand)
			{
				await ctx.ElevatedRespondAsync("Sorry! that command has been disabled in this guild.");
			}

			return true;
		}

		public static async Task DescribeCommandErrorAsync(Exception ex, string cmd, CommandContext ctx)
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

				var response = $"I couldn't execute `{cmd}` because ";
				response += string.Join(" and ", reasons);
				response += "!";
				await ctx.SafeRespondUnformattedAsync(response);
				return;
			}
			await ctx.SafeRespondAsync($"**Command {cmd} Errored!**\n{ex.Message}");
		}
	}
}