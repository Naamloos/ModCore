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
using DSharpPlus.ModernEmbedBuilder;
using F23.StringSimilarity;
using Humanizer;
using ModCore.Entities;
using ModCore.Logic;
using MoreLinq;
using System.Text;

namespace ModCore.Listeners
{
    public class ErrorLog
    {
        private static IEnumerable<(string name, Command cmd)> CommandSelector(KeyValuePair<string, Command> c)
        {
            return CommandSelector(c.Value);
        }

        private static IEnumerable<(string, Command)> CommandSelector(Command c)
        {
            var arr = new[] {(c.QualifiedName, c)};
            if (c is CommandGroup group)
            {
                return arr.Concat(group.Children.SelectMany(CommandSelector));
            }
            return arr;
        }

        [AsyncListener(EventTypes.CommandErrored)]
        public static async Task CommandError(ModCoreShard bot, CommandErrorEventArgs e)
        {
            var cfg = e.Context.GetGuildSettings();
            var ce = cfg.CommandError;
            var ctx = e.Context;

            if (e.Exception is CommandNotFoundException commandNotFound)
            {
                // return instead of proceeding, since qualifiedName below this will throw
                // (since there is no Command obj) 
                if (!cfg.SpellingHelperEnabled) return;
                
                // TODO: this only gives the first invalid token, so "a b" will just recognize as "a".
                //       it should probably take into consideration all the tokens.
                var attemptedName = commandNotFound.CommandName;
                try
                {
                    var commands = bot.Commands.RegisteredCommands.SelectMany(CommandSelector);

                    // TODO: i intended on using the library for more than just this,
                    //       but i ended up using it like this, so can probably just copy code from that lib 
                    //       instead of nugetting it
                    var leveshtein = new Levenshtein(); // lower is better

                    var ordered = commands
                        .Where(c => !(c.cmd is CommandGroup group) || group.IsExecutableWithoutSubcommands)
                        .Select(c => (qualifiedName: c.cmd.QualifiedName, description: c.cmd.Description))
                        .OrderBy(c => leveshtein.Distance(attemptedName, c.qualifiedName))
                        .DistinctBy(c => c.qualifiedName).Take(5).ToArray();

                    await new ModernEmbedBuilder
                    {
                        Title = "Command **" + attemptedName.Truncate(200) + "** not found",
                        Description = "Did you mean...",
                        Fields = ordered.Select(c =>
                            new DuckField(c.qualifiedName.Truncate(256), c.description.Truncate(999))).ToList()
//                        {
//                            (ordered[0].qualifiedName.Truncate(256), ordered[0].description.Truncate(999)),
//                            (ordered[1].qualifiedName.Truncate(256), ordered[1].description.Truncate(999)),
//                            (ordered[2].qualifiedName.Truncate(256), ordered[2].description.Truncate(999)),
//                        }
                    }.Send(ctx.Channel);
                }
                catch (Exception ex)
                {
                    e.Context.Client.DebugLogger.LogMessage(LogLevel.Critical, "CommandErrored", ex.ToString(),
                        DateTime.Now);
                }
                return;
            }

            var qualifiedName = e.Command.QualifiedName;
            switch (ce.Chat)
            {
                default:
                case CommandErrorVerbosity.None:
                    break;

                case CommandErrorVerbosity.Name:
                    await ctx.SafeRespondAsync($"**Command {qualifiedName} Errored!**");
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
                switch (ce.ActionLog)
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

		public static async Task DescribeCommandErrorAsync(Exception ex, string cmd, CommandContext ctx)
		{
			if(ex.GetType() == typeof(ChecksFailedException))
			{
				var reasons = new List<string>();

				var ext = (DSharpPlus.CommandsNext.Exceptions.ChecksFailedException)ex;
				var failed = ext.FailedChecks;

				if(failed.Any(x => x.GetType() == typeof(RequireUserPermissionsAttribute)))
					reasons.Add("you don't have the right permissions to execute this command");
				if (failed.Any(x => x.GetType() == typeof(RequireRolesAttribute)))
					reasons.Add("you don't have the right roles to execute this command");
				if (failed.Any(x => x.GetType() == typeof(RequirePrefixesAttribute)))
					reasons.Add("you can't execute this command with that prefix");
				if (failed.Any(x => x.GetType() == typeof(RequirePermissionsAttribute)))
					reasons.Add("one of us doesn't have the right permissions to execute this command");
				if (failed.Any(x => x.GetType() == typeof(RequireOwnerAttribute)))
					reasons.Add("you don't own this bot");
				if (failed.Any(x => x.GetType() == typeof(RequireNsfwAttribute)))
					reasons.Add("this command can only be executed in NSFW channels");
				if (failed.Any(x => x.GetType() == typeof(RequireBotPermissionsAttribute)))
					reasons.Add("I don't have the right permissions to execute this command");

				var response = $"I couldn't execute `{cmd}` because ";
				response += string.Join(" and ", reasons);
				response += "!";
				await ctx.SafeRespondAsync(response);
				return;
			}
			await ctx.SafeRespondAsync($"**Command {cmd} Errored!**\n{ex.Message}");
		}
    }
}