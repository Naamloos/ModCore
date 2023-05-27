using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Caching.Memory;
using ModCore.Database;
using ModCore.Database.DatabaseEntities;
using ModCore.Database.JsonEntities;
using ModCore.Entities;
using ModCore.Extensions;
using ModCore.Listeners;
using ModCore.Modals;
using ModCore.Utils;
using ModCore.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Commands
{
    [GuildOnly]
    public class Moderation : ApplicationCommandModule
    {
        public DatabaseContextBuilder Database { private get; set; }
        public SharedData Shared { private get; set; }

        public IMemoryCache Cache { private get; set; }

        [SlashCommand("offtopic", "Moves off-topic chat to another channel.")]
        [SlashCommandPermissions(Permissions.ManageMessages)]
        public async Task OfftopicAsync(InteractionContext ctx,
            [Option("channel", "Channel to copy last messages to.")] DiscordChannel channel,
            [Option("limit", "Maximum amount of messages to copy")] string limit = "20")
        {
            if (!channel.PermissionsFor(ctx.Guild.CurrentMember).HasPermission(Permissions.ManageMessages)
                && !channel.PermissionsFor(ctx.Member).HasPermission(Permissions.ManageMessages))
            {
                await ctx.CreateResponseAsync($"⚠️ Either of us does not have `MANAGE_MESSAGES` permission in {channel.Mention}!", true);
                return;
            }

            if (!int.TryParse(limit, out var parsedLimit) && parsedLimit < 1)
            {
                await ctx.CreateResponseAsync($"⚠️ {limit} is not a valid limit value!", true);
                return;
            }

            if (parsedLimit > 25)
            {
                await ctx.CreateResponseAsync($"⚠️ The maximum limit for off-topic messages is 25!", true);
                return;
            }

            IEnumerable<DiscordMessage> messages = await ctx.Channel.GetMessagesAsync(parsedLimit);

            await ctx.CreateResponseAsync($"⚠️ Your current conversation is off-topic! The most recent {parsedLimit} messages have been copied to {channel.Mention}.");

            await channel.SendMessageAsync($"⚠️ Copying off-topic messages from {ctx.Channel.Mention}!");
            var webhook = await channel.CreateWebhookAsync($"offtopic-move-{new Random().Next()}");

            foreach (var message in messages.Reverse())
            {
                if (string.IsNullOrEmpty(message.Content))
                    continue;

                var webhookMessage = new DiscordWebhookBuilder()
                    .WithContent(message.Content)
                    .WithAvatarUrl(message.Author.GetAvatarUrl(ImageFormat.Auto))
                    .WithUsername((message.Author as DiscordMember).DisplayName);

                await webhook.ExecuteAsync(webhookMessage);
            }
            await webhook.DeleteAsync();
            await channel.SendMessageAsync($"⚠ Off topic chat has been copied from {ctx.Channel.Mention}! Please continue conversation here.");
        }

        [SlashCommand("tempban", "Temporarily bans a member.")]
        [SlashCommandPermissions(Permissions.BanMembers)]
        public async Task TempbanAsync(InteractionContext ctx,
            [Option("user", "User to temporarily ban.")] DiscordUser user,
            [Option("unban_in", "When to unban this user.")] string unban_in,
            [Option("reason", "Reason why this member was banned.")] string reason = "")
        {
            if (ctx.User.Id == user.Id)
            {
                await ctx.CreateResponseAsync("⚠️ You can't do that to yourself! You have so much to live for!", true);
                return;
            }
            var member = await ctx.Guild.GetMemberAsync(user.Id);
            var (timespan, _) = Dates.ParseTime(unban_in);

            var unbanmoment = DateTimeOffset.UtcNow.Add(timespan);

            var userstring = $"{ctx.User.GetDisplayUsername()} ({ctx.User.Id})";
            var reasonstring = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
            var sent_dm = false;
            try
            {
                await member.SendMessageAsync($"🚓 You've been temporarily banned from {ctx.Guild.Name}" +
                    $"{(string.IsNullOrEmpty(reason) ? "." : $" with the following reason:\n```\n{reason}\n```")}" +
                    $"\nYou can rejoin <t:{unbanmoment.ToUnixTimeSeconds()}:R>");
                sent_dm = true;
            }
            catch (Exception) { }

            await member.BanAsync(7, $"{userstring}{reasonstring}");
            // Add timer
            var currentTime = DateTimeOffset.UtcNow;
            var dispatchTime = currentTime + timespan;

            var reminder = new DatabaseTimer
            {
                GuildId = (long)ctx.Guild.Id,
                ChannelId = 0,
                UserId = (long)member.Id,
                DispatchAt = dispatchTime.LocalDateTime,
                ActionType = TimerActionType.Unban
            };

            reminder.SetData(new TimerUnbanData
            {
                Discriminator = "",
                DisplayName = member.GetDisplayUsername(),
                UserId = (long)member.Id
            });

            using (var db = this.Database.CreateContext())
            {
                db.Timers.Add(reminder);
                await db.SaveChangesAsync();
            }

            await Timers.ScheduleNextAsync();

            var banEnd = DateTimeOffset.UtcNow.Add(timespan);

            // End of Timer adding
            await ctx.CreateResponseAsync($"🚓 Tempbanned user {member.DisplayName} (ID:{member.Id}) to be unbanned <t:{banEnd.ToUnixTimeSeconds()}:R>." +
                $"\n{(sent_dm ? "Said user has been notified of this action." : "")}", true);
        }

        [SlashCommand("massban", "Mass bans a group of members by ID.")]
        [SlashCommandPermissions(Permissions.BanMembers)]
        public async Task MassBanAsync(InteractionContext ctx)
        {
            await ctx.Client.GetInteractionExtension()
                .RespondWithModalAsync<MassBanModal>(ctx.Interaction, "Mass ban users");
        }

        [SlashCommand("hackban", "Bans a user by ID. This user does not have to be part of this server.")]
        [SlashCommandPermissions(Permissions.BanMembers)]
        public async Task HackbanAsync(InteractionContext ctx,
            [Option("userid", "ID of the user to ban.")] string userId,
            [Option("reason", "Reason to ban this user.")] string reason = null)
        {
            if (!ulong.TryParse(userId, out var id))
            {
                await ctx.CreateResponseAsync("⚠️ Invalid ID!", true);
                return;
            }

            try
            {
                await ctx.Guild.BanMemberAsync(id, 7, reason);
                await ctx.CreateResponseAsync($"🚓 User with ID {userId} was banned.", true);
            }
            catch (Exception)
            {
                await ctx.CreateResponseAsync($"⚠️ Failed to ban user with ID {userId}.", true);
            }
        }

        [SlashCommand("softban", "Bans and immediately unbans a member from this server. Deletes their messages up until a week ago.")]
        [SlashCommandPermissions(Permissions.BanMembers)]
        public async Task SoftbanAsync(InteractionContext ctx,
            [Option("user", "User to softban.")] DiscordUser user,
            [Option("reason", "Reason this user was soft banned.")] string reason = null)
        {
            var member = await ctx.Guild.GetMemberAsync(user.Id);
            if (ctx.User.Id == member.Id)
            {
                await ctx.CreateResponseAsync("⚠️ You can't do that to yourself! You have so much to live for!", true);
                return;
            }

            var userstring = $"{ctx.User.GetDisplayUsername()} ({ctx.User.Id})";
            var reasonstring = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
            var sent_dm = false;
            try
            {
                await member.SendMessageAsync($"🚓 You've been kicked from {ctx.Guild.Name}" +
                    $"{(string.IsNullOrEmpty(reason) ? "." : $" with the following reason:\n```\n{reason}\n```")}");
                sent_dm = true;
            }
            catch (Exception) { }

            await member.BanAsync(7, $"{userstring}{reasonstring} (softban)");
            await member.UnbanAsync(ctx.Guild, $"{userstring}{reasonstring}");
            await ctx.CreateResponseAsync($"🚓 Softbanned user {member.DisplayName} (ID:{member.Id}).\n{(sent_dm ? "Said user has been notified of this action." : "")}", true);
        }

        [SlashCommand("isolate", "Creates a new private thread with a user for moderation.")]
        [SlashCommandPermissions(Permissions.ManageThreads | Permissions.CreatePrivateThreads | Permissions.KickMembers)]
        public async Task IsolateAsync(InteractionContext ctx,
            [Option("user", "User to isolate.")] DiscordUser user,
            [Option("reason", "Reason to isolate said member.")] string reason = null,
            [Option("user_2", "Add another user to isolate")] DiscordUser user2 = null,
            [Option("user_3", "Add another user to isolate")] DiscordUser user3 = null,
            [Option("user_4", "Add another user to isolate")] DiscordUser user4 = null,
            [Option("user_5", "Add another user to isolate")] DiscordUser user5 = null)
        {
            List<DiscordUser> users = new List<DiscordUser>();
            users.Add(user);
            users.Add(user2);
            users.Add(user3);
            users.Add(user4);
            users.Add(user5);
            users.RemoveAll(x => x == null);

            foreach (var cuser in users)
            {
                if (!(cuser as DiscordMember).PermissionsIn(ctx.Channel).HasPermission(Permissions.AccessChannels))
                {
                    await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                        .WithContent($"⛔ One of the selected members can not access this channel. Try executing the command somewhere else!")
                        .AsEphemeral());
                    return;
                }
            }

            var mentions = string.Join(", ", users.Select(x => x.Mention));
            var names = string.Join(", ", users.Select(x => x.Username));

            var thread = await ctx.Channel.CreateThreadAsync($"⚠ {names}", AutoArchiveDuration.Day, ChannelType.PrivateThread, reason);
            await thread.SendMessageAsync($"⚠ {mentions}, a moderator has created an isolated chat with you" +
                $"{(reason != null ? $" for the following reasoning:\n```\n{reason}\n```" : ".")}" +
                $"_Said moderator can bring in more members through_ ***@pinging*** _when needed._");

            // Add the responsible moderator through a ping
            await (await thread.SendMessageAsync(ctx.Member.Mention)).DeleteAsync();

            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent($"✅ Created a new private thread: {thread.Mention}")
                .AsEphemeral());
        }

        [SlashCommand("unsnipe", "Clears message deletion/update data for this channel.")]
        [SlashCommandPermissions(Permissions.ManageMessages)]
        public async Task UnsnipeAsync(InteractionContext ctx)
        {
            var snipeId = $"snipe_{ctx.Channel.Id}";
            var eSnipeId = "e" + snipeId;

            Cache.Remove(snipeId);
            Cache.Remove(eSnipeId);

            await ctx.CreateResponseAsync($"Deletion and Edit snipe data for this channel was deleted!");
        }
    }
}