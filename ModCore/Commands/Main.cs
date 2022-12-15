using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using ModCore.Entities;
using ModCore.Extensions;
using ModCore.Utils.Extensions;
using ModCore.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using ModCore.Database;
using ModCore.Database.JsonEntities;
using System.Runtime.InteropServices;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using ModCore.AutoComplete;

namespace ModCore.Commands
{
    [GuildOnly]
    public class Main : ApplicationCommandModule
    {
        public SharedData Shared { private get; set; }
        public StartTimes StartTimes { private get; set; }
        public Settings Settings { private get; set; }
        public DatabaseContextBuilder Database { private get; set; }

        public IMemoryCache Cache { private get; set; }

        [SlashCommand("about", "Prints information about ModCore.")]
        public async Task AboutAsync(InteractionContext ctx)
        {
            var eb = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#089FDF"))
                .WithAuthor("ModCore", null, ctx.Client.CurrentUser.AvatarUrl)
                .WithDescription("ModCore is a powerful moderating bot written in C# using DSharpPlus.")
                .AddField("Main developer", "[Naamloos](https://github.com/Naamloos)")
                .AddField("Special thanks to all of these wonderful contributors:",
                    "[uwx](https://github.com/uwx), " +
                    "[jcryer](https://github.com/jcryer), " +
                    "[Emzi0767](https://github.com/Emzi0767), " +
                    "[YourAverageBlackGuy](https://github.com/YourAverageBlackGuy), " +
                    "[DrCreo](https://github.com/DrCreo), " +
                    "[aexolate](https://github.com/aexolate), " +
                    "[Drake103](https://github.com/Drake103), " +
                    "[Izumemori](https://github.com/Izumemori) and " +
                    "[OoLunar](https://github.com/OoLunar)")
                .AddField("Want to contribute?", "Contributions are always welcome at our [GitHub repo.](https://github.com/Naamloos/ModCore)")
                .AddField("Donate?", "Currently, ModCore is hosted off my (Naamloos's) own money. Donations are always welcome over at [Ko-Fi](https://ko-fi.com/Naamloos)!")
                .WithThumbnail(ctx.Client.CurrentUser.AvatarUrl)
                .Build();

            await ctx.CreateResponseAsync(eb, true);
        }

        [SlashCommand("avatar", "Fetches a user's avatar with URL.")]
        public async Task AvatarAsync(InteractionContext ctx, [Option("user", "User to fetch the avatar from.")] DiscordUser user)
        {
            var img = user.GetAvatarUrl(ImageFormat.Png, 4096);

            var embed = new DiscordEmbedBuilder()
                .WithTitle($"Avatar for user {user.Username}")
                .WithDescription(img)
                .WithImageUrl(img);

            await ctx.CreateResponseAsync(embed, true);
        }

        [SlashCommand("status", "Returns ModCore status info.")]
        public async Task StatusAsync(InteractionContext ctx)
        {
            var osVersion = Environment.OSVersion.VersionString;
            var embed = new DiscordEmbedBuilder()
                .WithTitle("ModCore Status")
                .WithDescription("Information about ModCore's status.")
                .WithColor(new DiscordColor("#089FDF"))
                .WithThumbnail(ctx.Client.CurrentUser.GetAvatarUrl(ImageFormat.Png))
                .AddField("🏓 Socket Ping", $"{ctx.Client.Ping} ms", true)
                .AddField("🏠 Servers", $"{this.Shared.ModCore.Shards.Select(x => x.Client.Guilds.Count).Sum()}", true)
                .AddField("🏙 Shards", $"{ctx.Client.ShardCount}", true)
                .AddField("👋 Current Shard", $"{ctx.Client.ShardId}", true)
                .AddField("⏱️ Program Uptime", string.Format("<t:{0}:R>", StartTimes.ProcessStartTime.ToUnixTimeSeconds()), true)
                .AddField("⏱️ Socket Uptime", string.Format("<t:{0}:R>", StartTimes.SocketStartTime.ToUnixTimeSeconds()), true)
                .AddField("💻 Operating System", osVersion.StartsWith("Unix")? fetchLinuxName() : Environment.OSVersion.VersionString, true)
                .AddField("🪟 Framework", RuntimeInformation.FrameworkDescription, true)
                .AddField("📖 DSharpPlus", ctx.Client.VersionString, true);

            await ctx.CreateResponseAsync(embed, true);
        }

        static Regex prettyNameRegex = new Regex("PRETTY_NAME=(.*)", RegexOptions.Compiled);
        private string fetchLinuxName()
        {
            try
            {
                var result = File.ReadAllText("/etc/os-release");
                var match = prettyNameRegex.Match(result);
                if (!match.Success)
                    return Environment.OSVersion.VersionString;
                return match.Groups[1].Value.Replace("\"", "");
            }catch(Exception)
            {
                return Environment.OSVersion.VersionString;
            }
        }

        [SlashCommand("snipe", "Retrieves the last deleted message from cache.")]
        public Task SnipeAsync(InteractionContext ctx)
            => doSnipeAsync(ctx, false);

        [SlashCommand("snipeedit", "Retrieves the last edited message's original content from cache.")]
        public Task SnipeEditAsync(InteractionContext ctx)
            => doSnipeAsync(ctx, true);

        private async Task doSnipeAsync(InteractionContext ctx, bool edit)
        {
            var snipeId = $"{(edit? "esnipe" : "snipe")}_{ctx.Channel.Id}";

            if(Cache.TryGetValue(snipeId, out DiscordMessage message))
            {
                var content = message.Content;
                if (content.Length > 500)
                    content = content.Substring(0, 500) + "...";

                var embed = new DiscordEmbedBuilder()
                    .WithAuthor($"{message.Author.Username}#{message.Author.Discriminator}" + (edit ? " (Edited)" : ""),
                        iconUrl: message.Author.GetAvatarUrl(ImageFormat.Png))
                    .WithFooter($"{(edit? "Edit" : "Deletion")} sniped by {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl);

                if (!string.IsNullOrEmpty(content))
                {
                    embed.WithDescription(content);
                }

                embed.WithTimestamp(message.Id);

                List<DiscordEmbedBuilder> embeds = new List<DiscordEmbedBuilder>();
                var attachments = message.Attachments.Where(x => x.MediaType.StartsWith("image/"));

                for (int i = 0; i < attachments.Count(); i++)
                {
                    var attachment = attachments.ElementAt(i);
                    if (i == 0)
                    {
                        embed.WithThumbnail(attachment.Url);
                    }
                    else
                    {
                        embeds.Add(new DiscordEmbedBuilder()
                            .WithTitle("Additional Image").WithThumbnail(attachment.Url));
                    }
                }
                
                var response = new DiscordInteractionResponseBuilder()
                    .AddEmbeds(embeds.Prepend(embed).Select(x => x.Build()));

                var customId = ExtensionStatics.GenerateIdString("del", new Dictionary<string, string>()
                {
                    {"u", ctx.User.Id.ToString() + "|" + message.Author.ToString() },
                });

                response.AddComponents(new DiscordButtonComponent(ButtonStyle.Danger, customId, "", emoji: new DiscordComponentEmoji("🗑")));

                await ctx.CreateResponseAsync(response);
                return;
            }

            await ctx.CreateResponseAsync("⚠️ No message to snipe! Either nothing was deleted, or the message has expired (12 hours)!", true);
        }

        [SlashCommand("contact", "Contact ModCore developers.")]
        public async Task ContactAsync(InteractionContext ctx)
        {
            await ctx.DeferAsync(true);

            var msg = new DiscordFollowupMessageBuilder()
                .WithContent("❓ Which category does your support request fit in?")
                .AsEphemeral();

            var response = await ctx.MakeEnumChoiceAsync<FeedbackType>(msg);

            if (response.TimedOut)
            {
                await ctx.EditFollowupAsync(response.FollowupMessage.Id, new DiscordWebhookBuilder().WithContent("🙈 Okay, never mind."));
                return;
            }

            await ctx.EditFollowupAsync(response.FollowupMessage.Id, new DiscordWebhookBuilder().WithContent("Thank you for your feedback! 💖"));

            var feedbackTypeString = Enum.GetName(response.Choice.GetType(), response.Choice);
            await ctx.Client.GetInteractionExtension().RespondWithModalAsync<FeedbackModal>(response.interaction,
                $"Feedback: {feedbackTypeString}", new Dictionary<string, string>()
                {
                    { "c", feedbackTypeString }
                });
        }

        [SlashCommand("nick", "Requests nickname change, if nickname change is disabled")]
        public async Task NicknameAsync(InteractionContext ctx, [Option("nickname", "New nickname you want to request.")] string nickname)
        {
            if (nickname.Length < 1 || nickname.Length > 32)
            {
                await ctx.CreateResponseAsync("⚠️ Nicknames have to be at least 1 character and at most 32 characters long!", true);
                return;
            }
            var member = await ctx.Guild.GetMemberAsync(ctx.User.Id);

            if (nickname == member.Nickname)
            {
                await ctx.CreateResponseAsync("⚠️ That's already your nickname.", true);
                return;
            }

            // attempt to automatically change the person's nickname if they can already change it on their own,
            // and prompt them if we're not able to
            if (member.PermissionsIn(ctx.Channel).HasPermission(Permissions.ChangeNickname))
            {
                if (ctx.Guild.CurrentMember.PermissionsIn(ctx.Channel).HasPermission(Permissions.ManageNicknames) &&
                    ctx.Guild.CurrentMember.CanInteract(ctx.Member))
                {
                    await member.ModifyAsync(member =>
                    {
                        member.Nickname = nickname;
                        member.AuditLogReason = "Nickname change requested by " +
                                                "@{reaction.User.Username}#{reaction.User.Discriminator} auto approved " +
                                                "since they already have the Change Nickname permission";
                    });
                    await ctx.CreateResponseAsync("✅ Your nickname change was auto-approved since you have the right permissions!", true);
                    return;
                }

                await ctx.CreateResponseAsync("⚠️ Do it yourself, you have the permission!", true);
                return;
            }

            var config = ctx.Guild.GetGuildSettings(Database.CreateContext()) ?? new GuildSettings();

            // don't change the member's nickname here, as that violates the hierarchy of permissions
            if (!config.NicknameConfirm.Enable)
            {
                if (member == ctx.Guild.Owner)
                {
                    await ctx.CreateResponseAsync("⚠️ Use the `config` command to enable nickname " +
                                                   "change requests.", true);
                }
                else
                {
                    await ctx.CreateResponseAsync("⚠️ The server owner has disabled nickname changing on this server.", true);
                }

                return;
            }

            // only say it's unable to process if BOTH the confirmation requirement is enabled and the bot doesn't
            // have the permissions for it
            if (!ctx.Guild.CurrentMember.PermissionsIn(ctx.Channel).HasPermission(Permissions.ManageNicknames) ||
                !ctx.Guild.CurrentMember.CanInteract(member))
            {
                await ctx.CreateResponseAsync("⚠️ Unable to process nickname change because the bot lacks the " +
                                               "required permissions, or cannot action on this member.", true);
                return;
            }

            var channel = ctx.Guild.GetChannel(config.NicknameConfirm.ChannelId);

            if (channel == null)
            {
                await ctx.CreateResponseAsync(
                    "⛔ This server has not correctly configured nickname approval. **Ask the server owner to correctly configure the approval channel!**", true);
                return;
            }

            // d#+ nightlies mean we can do this now, and hopefully it won't crash
            await ctx.CreateResponseAsync(
                "✅ Your request to change your nickname was placed, and should be actioned shortly.", true);

            var embed = new DiscordEmbedBuilder()
                .WithAuthor(member.Username + "#" + member.Discriminator, iconUrl: member.GetAvatarUrl(ImageFormat.Png))
                .WithDescription($"Nickname request")
                .AddField("New nickname", nickname)
                .AddField("Old nickname", member.DisplayName)
                .WithColor(DiscordColor.LightGray);

            var approve = ExtensionStatics.GenerateIdString("nick.yes", new Dictionary<string, string>()
            {
                { "n", nickname },
                { "u", member.Id.ToString() }
            });

            var deny = ExtensionStatics.GenerateIdString("nick.no", new Dictionary<string, string>()
            {
                { "n", nickname },
                { "u", member.Id.ToString() }
            });

            var msg = new DiscordMessageBuilder()
                .AddEmbed(embed)
                .AddComponents(
                    new DiscordButtonComponent(ButtonStyle.Success, approve, "Approve", emoji: new DiscordComponentEmoji(DiscordEmoji.FromUnicode("✏️"))),
                    new DiscordButtonComponent(ButtonStyle.Danger, deny, "Deny", emoji: new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🗑️")))
                    );

            await channel.SendMessageAsync(msg);
        }

        [SlashCommand("coinflip", "Flips a coin.")]
        public async Task FlipCoinAsync(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync($"Alright! flipping a coin for you...");
            await Task.Delay(1000);
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"​ㅤ\n✊🪙"));
            await Task.Delay(200);
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"​ㅤㅤ🪙ㅤㅤ\n☝️✨"));
            await Task.Delay(200);
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"​ㅤㅤ✨🪙ㅤ\n☝️✨"));
            await Task.Delay(200);
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"ㅤㅤ✨✨\n​☝️✨ㅤㅤ🪙"));
            await Task.Delay(1000);
            var rng = new Random().Next(0, 2);
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"​🤔 Hmm... Looks like it landed on {(rng == 0? "heads" : "tails")}!"));
        }

        private static Regex SlashCommandRegex = new Regex(@"</[-_\p{L}\p{N} ]{1,32}:([0-9]+)>", RegexOptions.Compiled);
        [SlashCommand("command", "Sends a command mention to chat")]
        public async Task CommandAsync(InteractionContext ctx,
            [Autocomplete(typeof(SlashCommandAutoComplete))]
            [Option("command", "Command to mention", true)]
                string command)
        {
            if(SlashCommandRegex.IsMatch(command))
                await ctx.CreateResponseAsync(command, false);
            else
                await ctx.CreateResponseAsync($"⚠️ {command} is not a valid command!", true);
        }
    }
}
