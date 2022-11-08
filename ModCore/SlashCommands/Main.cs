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

namespace ModCore.SlashCommands
{
    public class Main : ApplicationCommandModule
    {
        public SharedData Shared { private get; set; }
        public StartTimes StartTimes { private get; set; }
        public Settings Settings { private get; set; }
        public DatabaseContextBuilder Database { private get; set; }

        [SlashCommand("about", "Prints information about ModCore.")]
        public async Task AboutAsync(InteractionContext ctx)
        {
            var eb = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#089FDF"))
                .WithTitle("ModCore")
                .WithDescription("A powerful moderating bot written on top of DSharpPlus")
                .AddField("Main developer", "[Naamloos](https://github.com/Naamloos)")
                .AddField("Special thanks to these contributors:",
                    "[uwx](https://github.com/uwx), " +
                    "[jcryer](https://github.com/jcryer), " +
                    "[Emzi0767](https://github.com/Emzi0767), " +
                    "[YourAverageBlackGuy](https://github.com/YourAverageBlackGuy), " +
                    "[DrCreo](https://github.com/DrCreo), " +
                    "[aexolate](https://github.com/aexolate), " +
                    "[Drake103](https://github.com/Drake103) and " +
                    "[Izumemori](https://github.com/Izumemori)")
                .AddField("Environment",
                    $"*OS:* {Environment.OSVersion.VersionString}" +
                    $"\n*Framework:* {Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName}" +
                    $"\n*DSharpPlus:* {ctx.Client.VersionString}" +
                    $"\n*Servers:* {this.Shared.ModCore.Shards.Select(x => x.Client.Guilds.Count).Sum()}" +
                    $"\n*Shards:* {this.Shared.ModCore.Shards.Count}")
                .AddField("Contribute?", "Contributions are always welcome at our [GitHub repo.](https://github.com/Naamloos/ModCore)")
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
            var embed = new DiscordEmbedBuilder()
                .WithTitle("ModCore Status")
                .WithDescription("Information about ModCore's status.")
                .WithThumbnail(ctx.Client.CurrentUser.GetAvatarUrl(ImageFormat.Png))
                .AddField("🏓 Socket Ping", $"{ctx.Client.Ping} ms", true)
                .AddField("⚡ Servers", $"{this.Shared.ModCore.Shards.Select(x => x.Client.Guilds.Count).Sum()}", true)
                .AddField("⚡ Shards", $"{ctx.Client.ShardCount}", true)
                .AddField("⚡ Current Shard", $"{ctx.Client.ShardId}", true)
                .AddField("⏱️ Program Uptime", string.Format("<t:{0}:R>", StartTimes.ProcessStartTime.ToUnixTimeSeconds()), true)
                .AddField("⏱️ Socket Uptime", string.Format("<t:{0}:R>", StartTimes.SocketStartTime.ToUnixTimeSeconds()), true);

            await ctx.CreateResponseAsync(embed, true);
        }

        [SlashCommand("snipe", "Retrieves the last deleted message from cache.")]
        public async Task SnipeAsync(InteractionContext ctx, [Option("edit", "Whether to fetch an edited message or a deleted one.")] bool edit = false)
        {
            var messages = edit ? this.Shared.EditedMessages : this.Shared.DeletedMessages;
            if (messages.ContainsKey(ctx.Channel.Id))
            {
                var message = this.Shared.DeletedMessages[ctx.Channel.Id];

                var content = message.Content;
                if (content.Length > 500)
                    content = content.Substring(0, 500) + "...";

                var embed = new DiscordEmbedBuilder()
                    .WithAuthor($"{message.Author.Username}#{message.Author.Discriminator}" + (edit ? " (Edited)" : ""),
                        iconUrl: message.Author.GetAvatarUrl(ImageFormat.Png));

                if (!string.IsNullOrEmpty(message.Content))
                {
                    embed.WithDescription(message.Content);
                    embed.WithTimestamp(message.Id);
                }

                if (message.Attachments.Count > 0)
                {
                    if (message.Attachments[0].MediaType == "image/png"
                        || message.Attachments[0].MediaType == "image/jpeg"
                        || message.Attachments[0].MediaType == "image/gif"
                        || message.Attachments[0].MediaType == "image/apng"
                        || message.Attachments[0].MediaType == "image/webp")
                        embed.WithImageUrl(message.Attachments[0].Url);
                }

                await ctx.CreateResponseAsync(embed);
                return;
            }

            await ctx.CreateResponseAsync("⚠️ No message to snipe!", true);
        }

        [SlashCommand("invite", "Get an invite to this ModCore instance. Sharing is caring!")]
        public async Task InviteAsync(InteractionContext ctx)
        {
            var app = ctx.Client.CurrentApplication;
            if (app.IsPublic != null && (bool)app.IsPublic)
                await ctx.CreateResponseAsync(
                    $"🛡 Add ModCore to your server!\n<https://modcore.naamloos.dev/info/invite>", true);
            else
                await ctx.CreateResponseAsync("⚠️ I'm sorry Mario, but this instance of ModCore has been set to private!", true);
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
            if(nickname.Length < 1 || nickname.Length > 32)
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
            if (!config.RequireNicknameChangeConfirmation)
            {
                if (member == ctx.Guild.Owner)
                {
                    await ctx.CreateResponseAsync("⚠️ Use the `config nickchange enable` command to enable nickname " +
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

            await ctx.Guild.GetChannel(config.NicknameChangeConfirmationChannel).SendMessageAsync(msg);
        }
    }
}
