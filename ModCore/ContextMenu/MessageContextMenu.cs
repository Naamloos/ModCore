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
using DeepL;
using System;
using System.Reflection;
using ModCore.Entities;

namespace ModCore.ContextMenu
{
    [GuildOnly]
    public class MessageContextMenu : ApplicationCommandModule
    {
        // Message Context Menu commands here. Max 5.

        public InteractivityExtension interactivity { private get; set; }

        public Settings settings { private get; set; }

        const string EMOJI_REGEX = @"<a?:(.+?):(\d+)>";

        [ContextMenu(ApplicationCommandType.MessageContextMenu, "Copy emoji")]
        [SlashCommandPermissions(Permissions.ManageEmojis)]
        [GuildOnly]
        public async Task YoinkAsync(ContextMenuContext ctx)
        {
            await ctx.DeferAsync(true);
            var msg = ctx.TargetMessage;

            if (msg != null)
            {
                var matches = Regex.Matches(msg.Content, EMOJI_REGEX).DistinctBy(x => x.Groups[1].Value);

                if (!matches.Any())
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
                    response = $"✅ Yoink! Emoji added to this server by <@{ctx.User.Id}>: {emoji.ToString()}";

                    if (ctx.Channel.PermissionsFor(ctx.Guild.CurrentMember).HasPermission(Permissions.SendMessages))
                    {
                        await ctx.Interaction.DeleteOriginalResponseAsync();
                        await ctx.Channel.SendMessageAsync(new DiscordMessageBuilder().WithContent(response).WithReply(ctx.TargetMessage.Id, false, false));
                        
                        return;
                    }
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

        [ContextMenu(ApplicationCommandType.MessageContextMenu, "Copy sticker")]
        [SlashCommandPermissions(Permissions.ManageEmojis)]
        [GuildOnly]
        public async Task YoinkStickerAsync(ContextMenuContext ctx)
        {
            await ctx.DeferAsync(true);
            var msg = ctx.TargetMessage;

            if (msg != null)
            {
                var stickers = ctx.TargetMessage.Stickers.Where(x => x.FormatType != StickerFormat.LOTTIE);

                if (!stickers.Any())
                {
                    await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("⚠️ Referenced sticker not found! Do note that ModCore can not copy LOTTIE type stickers!"));
                    return;
                }

                var sticker = stickers.First();
                DiscordMessage followup = null;

                if (stickers.Count() > 1)
                {
                    var message = new DiscordFollowupMessageBuilder()
                        .WithContent("Which sticker do you want to copy?")
                        .AsEphemeral();

                    var choices = new Dictionary<string, DiscordMessageSticker>();
                    foreach (DiscordMessageSticker currentSticker in stickers)
                    {
                        choices.Add(currentSticker.Name, currentSticker);
                    }

                    var selection = await ctx.MakeChoiceAsync<DiscordMessageSticker>(message, choices);
                    if (selection.TimedOut)
                    {
                        await ctx.EditFollowupAsync(selection.FollowupMessage.Id, new DiscordWebhookBuilder().WithContent("⚠️ Timed out selection."));
                        return;
                    }

                    followup = selection.FollowupMessage;
                    sticker = selection.choice;
                }

                var newSticker = await stealieSticker(ctx.Guild, sticker);
                var response = $"✅ Yoink! Sticker added to this server by <@{ctx.User.Id}>: {newSticker.Name}";

                if (ctx.Channel.PermissionsFor(ctx.Guild.CurrentMember).HasPermission(Permissions.SendMessages))
                {
                    await ctx.Interaction.DeleteOriginalResponseAsync();
                    await ctx.Channel.SendMessageAsync(new DiscordMessageBuilder().WithContent(response).WithReply(ctx.TargetMessage.Id, false, false).WithSticker(newSticker));

                    return;
                }


                if (followup == null)
                    await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(response));
                else
                    await ctx.EditFollowupAsync(followup.Id, new DiscordWebhookBuilder().WithContent(response));
            }
        }

        private async Task<DiscordMessageSticker> stealieSticker(DiscordGuild guild, DiscordMessageSticker sticker)
        {
            using HttpClient _client = new HttpClient();
            var downloadedEmoji = await _client.GetStreamAsync(sticker.StickerUrl);
            using MemoryStream memory = new MemoryStream();
            downloadedEmoji.CopyTo(memory);
            downloadedEmoji.Dispose();
            memory.Position = 0;
            return await guild.CreateStickerAsync(sticker.Name ?? "Sticker", sticker.Description ?? "", "🤡", memory, sticker.FormatType);
        }

        [ContextMenu(ApplicationCommandType.MessageContextMenu, "Translate (DeepL)")]
        [SlashCommandPermissions(Permissions.ReadMessageHistory)]
        [GuildOnly]
        public async Task TranslateAsync(ContextMenuContext ctx)
        {
            if(string.IsNullOrEmpty(settings.DeepLToken))
            {
                await ctx.CreateResponseAsync("No DeepL token configured! Notify the bot developers via /contact!", true);
                return;
            }

            var translate = ctx.TargetMessage.Content;
            var translator = new Translator(settings.DeepLToken);

            if(string.IsNullOrEmpty(translate))
            {
                await ctx.CreateResponseAsync("That message has no content!", true);
                return;
            }

            await ctx.DeferAsync(false);

            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resource = "ModCore.Assets.globe-showing-europe.gif";
                var iconAsset = assembly.GetManifestResourceStream(resource);

                var translation = await translator.TranslateTextAsync(translate, null, LanguageCode.EnglishAmerican);
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithDescription($"Translated from language: {translation.DetectedSourceLanguageCode} to {LanguageCode.EnglishAmerican}.")
                        .AddField("Original Text", translate)
                        .AddField("Translated Text", translation.Text)
                        .WithColor(new DiscordColor("09a0e2"))
                        .WithThumbnail("attachment://globe-showing-europe.gif"))
                    .AddComponents(new DiscordLinkButtonComponent(ctx.TargetMessage.JumpLink.ToString(), "Jump to Original", emoji: new DiscordComponentEmoji("💭")))
                    .AddFile("globe-showing-europe.gif", iconAsset));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
