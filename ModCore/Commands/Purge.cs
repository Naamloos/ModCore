using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using ModCore.Utils.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModCore.Commands
{
    [SlashCommandGroup("purge", "Commands for clearing chat.")]
    [SlashCommandPermissions(Permissions.ManageMessages)]
    [SlashRequirePermissions(Permissions.ManageMessages)]
    [GuildOnly]
    public class Purge : ApplicationCommandModule
    {
        // regular, user, regex, commands, bots, images
        [SlashCommand("regular", "Clears chat without any special parameters.")]
        public async Task RegularPurgeAsync(InteractionContext ctx, 
            [Option("limit", "Maximum amount of messages to fetch in this Purge")][Maximum(100)][Minimum(1)]long limit = 50,
            [Option("skip", "Amount of newer messages to skip when purging")][Minimum(0)][Maximum(99)]long skip = 0)
        {
            IEnumerable<DiscordMessage> messages = (await ctx.Channel.GetMessagesAsync((int)(limit + skip))).Skip((int)skip);

            await deleteAsync(ctx, messages);
        }

        [SlashCommand("user", "Clears chat by a specific user.")]
        public async Task UserPurgeAsync(InteractionContext ctx, 
            [Option("user", "User to delete messages from.")]DiscordUser user,
            [Option("limit", "Maximum amount of messages to fetch in this Purge")][Maximum(100)][Minimum(1)] long limit = 50,
            [Option("skip", "Amount of newer messages to skip when purging")][Minimum(0)][Maximum(99)] long skip = 0)
        {
            IEnumerable<DiscordMessage> messages = (await ctx.Channel.GetMessagesAsync((int)(limit + skip))).Skip((int)skip);
            messages = messages.Where(x => x.Author.Id == user.Id);

            await deleteAsync(ctx, messages);
        }

		[SlashCommand("regex", "Clears chat using a Regular Expression. (EXPERT USERS ONLY!)")]
        public async Task RegexPurgeAsync(InteractionContext ctx, 
            [Option("regex", "Regular Expression to use.")] string regex,
            [Option("flags", "Regex flags to use.")] string flags = "",
            [Option("limit", "Maximum amount of messages to fetch in this Purge")][Maximum(100)][Minimum(1)] long limit = 50,
            [Option("skip", "Amount of newer messages to skip when purging")][Minimum(0)][Maximum(99)] long skip = 0)
        {
			IEnumerable<DiscordMessage> messages = (await ctx.Channel.GetMessagesAsync((int)(limit + skip))).Skip((int)skip);

			// TODO add a flag to disable CultureInvariant.
			var regexOptions = RegexOptions.CultureInvariant;

			if (string.IsNullOrEmpty(regex))
			{
				await ctx.CreateResponseAsync("⚠️ Regex is empty!", true);
				return;
			}

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

			var regexCompiled = new Regex(regex, regexOptions);

			messages = messages.Where(x => regexCompiled.IsMatch(x.Content));
			await deleteAsync(ctx, messages);
		}

        [SlashCommand("bots", "Clears chat messages by bots.")]
        public async Task BotsPurgeAsync(InteractionContext ctx,
            [Option("limit", "Maximum amount of messages to fetch in this Purge")][Maximum(100)][Minimum(1)] long limit = 50,
            [Option("skip", "Amount of newer messages to skip when purging")][Minimum(0)][Maximum(99)] long skip = 0)
        {
            IEnumerable<DiscordMessage> messages = (await ctx.Channel.GetMessagesAsync((int)(limit + skip))).Skip((int)skip);
            messages = messages.Where(x => x.Author.IsBot);

            await deleteAsync(ctx, messages);
        }

        [SlashCommand("attachments", "Clears chat messages with attachments.")]
        public async Task AttachmentsPurgeAsync(InteractionContext ctx,
            [Option("limit", "Maximum amount of messages to fetch in this Purge")][Maximum(100)][Minimum(1)] long limit = 50,
            [Option("skip", "Amount of newer messages to skip when purging")][Minimum(0)][Maximum(99)] long skip = 0)
        {
            IEnumerable<DiscordMessage> messages = (await ctx.Channel.GetMessagesAsync((int)(limit + skip))).Skip((int)skip);
            messages = messages.Where(x => x.Attachments.Count > 0);

            await deleteAsync(ctx, messages);
        }

        private Regex ImageRegex = new Regex(@"\.(png|gif|jpg|jpeg|tiff|webp)");

        [SlashCommand("images", "Clears chat messages with attachments.")]
        public async Task ImagesPurgeAsync(InteractionContext ctx,
            [Option("limit", "Maximum amount of messages to fetch in this Purge")][Maximum(100)][Minimum(1)] long limit = 50,
            [Option("skip", "Amount of newer messages to skip when purging")][Minimum(0)][Maximum(99)] long skip = 0)
        {
            IEnumerable<DiscordMessage> messages = (await ctx.Channel.GetMessagesAsync((int)(limit + skip))).Skip((int)skip);
            messages = messages.Where(x => x.Attachments.Count > 0);
            messages = messages.Where(m => ImageRegex.IsMatch(m.Content) || m.Attachments.Any(x => ImageRegex.IsMatch(x.FileName)));
            await deleteAsync(ctx, messages);
        }

        private async Task deleteAsync(InteractionContext ctx, IEnumerable<DiscordMessage> messages)
        {
            var deleteable = messages;
            deleteable = deleteable.Where(x => DateTimeOffset.Now.Subtract(x.CreationTimestamp).TotalDays < 14).ToList();

            if(!deleteable.Any())
            {
                await ctx.CreateResponseAsync("⚠️ No messages were deleted. Take note that messages older than 14 days can not be deleted with purge.", true);
                return;
            }

            await ctx.Channel.DeleteMessagesAsync(deleteable, "ModCore Purge");
            await ctx.CreateResponseAsync($"✅ Deleted {deleteable.Count()} messages.", true);
        }
	}
}
