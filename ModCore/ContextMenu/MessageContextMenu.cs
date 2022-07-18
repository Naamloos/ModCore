using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ModCore.Utils.Extensions;
using System.Collections.Generic;
using DSharpPlus.Interactivity;
using System.Linq;

namespace ModCore.ContextMenu
{
    public class MessageContextMenu : ApplicationCommandModule
    {
        // Message Context Menu commands here. Max 5.

        public InteractivityExtension interactivity { private get; set; }

        const string EMOJI_REGEX = @"<a?:(.+?):(\d+)>";
        [ContextMenu(ApplicationCommandType.MessageContextMenu, "Copy emoji")]
        [SlashCommandPermissions(Permissions.ManageEmojis)]
        public async Task YoinkAsync(ContextMenuContext ctx)
        {
            await ctx.DeferAsync(true);
            var msg = ctx.TargetMessage;

            if (msg != null)
            {
                var matches = Regex.Matches(msg.Content, EMOJI_REGEX).DistinctBy(x => x.Groups[1].Value);

                if (matches.Count() < 1)
                {
                    await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("⚠️ Referenced emoji not found!"));
                    return;
                }

                var match = matches.First();
                DiscordMessage followup = null;

                if(matches.Count() > 1)
                {
                    var message = new DiscordFollowupMessageBuilder()
                        .WithContent("Which emoji do you want to copy?")
                        .AsEphemeral();

                    var choices = new Dictionary<string, Match>();
                    foreach (Match currentmatch in matches)
                    {
                        choices.Add(currentmatch.Groups[1].Value, currentmatch);
                    }

                    var selection = await ctx.MakeChoiceAsync<Match>(message, choices);
                    if(selection.TimedOut)
                    {
                        await ctx.EditFollowupAsync(selection.FollowupMessage.Id, new DiscordWebhookBuilder().WithContent("⚠️ Timed out selection."));
                        return;
                    }

                    followup = selection.FollowupMessage;
                    match = selection.choice;
                }

                var split = match.Groups[2].Value;
                var emojiName = match.Groups[1].Value;
                var animated = match.Value.StartsWith("<a");

                var response = "⚠️ Failed to fetch your new emoji.";

                if (ulong.TryParse(split, out ulong emoji_id))
                {
                    var emoji = await stealieEmoji(ctx.Guild, emojiName, emoji_id, animated);
                    response = $"✅ Yoink! Emoji added to this server: {emoji.ToString()}";
                }

                if (followup == null)
                    await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(response));
                else
                    await ctx.EditFollowupAsync(followup.Id, new DiscordWebhookBuilder().WithContent(response));
            }
        }

        private async Task<DiscordEmoji> stealieEmoji(DiscordGuild guild, string name, ulong id, bool animated)
        {
            using HttpClient _client = new HttpClient();
            var downloadedEmoji = await _client.GetStreamAsync($"https://cdn.discordapp.com/emojis/{id}.{(animated ? "gif" : "png")}");
            using MemoryStream memory = new MemoryStream();
            downloadedEmoji.CopyTo(memory);
            downloadedEmoji.Dispose();
            return await guild.CreateEmojiAsync(name, memory);
        }
    }
}
