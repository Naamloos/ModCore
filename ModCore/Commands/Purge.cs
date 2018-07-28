using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ModCore.Entities;
using ModCore.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ModCore.Logic.Extensions;

namespace ModCore.Commands
{
	[Group("purge"), Aliases("p"), RequirePermissions(Permissions.ManageMessages), CheckDisable]
	public class Purge : BaseCommandModule
	{
		private static readonly Regex SpaceReplacer = new Regex(" {2,}", RegexOptions.Compiled);

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

		[Command("user"), Description("Delete an amount of messages by an user."), Aliases("u", "pu"), CheckDisable]
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
		 "in quotes."), Aliases("purgeregex", "pr", "r"), CheckDisable]
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
	 RequirePermissions(Permissions.ManageMessages), CheckDisable]
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
	 RequirePermissions(Permissions.ManageMessages), CheckDisable]
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
	 RequirePermissions(Permissions.ManageMessages), CheckDisable]
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
	}
}
