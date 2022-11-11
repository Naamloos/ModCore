using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Utils;
using ModCore.Utils.Extensions;

namespace ModCore.LegacyCommands
{
    [Group("owner"), Aliases("o"), Hidden, RequireOwner]
    public class Owner : BaseCommandModule
	{
        public SharedData Shared { get; }
        public DatabaseContextBuilder Database { get; }

        public Owner(SharedData shared, DatabaseContextBuilder db)
        {
            this.Shared = shared;
            this.Database = db;
        }

        [Command("clear"), Hidden, RequireOwner]
        public async Task ClearCommandsAsync(CommandContext context)
        {
            await context.SafeRespondUnformattedAsync("❓ Are you sure you want to clear application commands? This will also shutdown the bot.");

            var cancellationtokensource = context.Services.GetService<SharedData>().CancellationTokenSource;
            var interactivity = context.Services.GetService<InteractivityExtension>();
            var message = await interactivity.WaitForMessageAsync(x => x.ChannelId == context.Channel.Id && x.Author.Id == context.Member.Id, TimeSpan.FromSeconds(30));

            if (message.TimedOut)
            {
                await context.SafeRespondUnformattedAsync("⚠️⌛ Timed out.");
            }
            else if (InteractivityUtil.Confirm(message.Result))
            {
                await context.Client.BulkOverwriteGlobalApplicationCommandsAsync(new List<DiscordApplicationCommand>());
                await context.SafeRespondUnformattedAsync("✅ Cleared commands, shutting down.");
                cancellationtokensource.Cancel(false);
            }
            else
            {
                await context.SafeRespondUnformattedAsync("✅ Operation cancelled by user.");
            }
        }

	    [Command("exit"), Aliases("e"), Hidden, RequireOwner]
        public async Task ExitAsync(CommandContext context)
        {
            if (!context.Client.CurrentApplication.Owners.Any(x => x.Id == context.User.Id))
            {
                await context.SafeRespondUnformattedAsync("⚠️ You do not have permission to use this command!");
                return;
            }

            await context.SafeRespondUnformattedAsync("❓ Are you sure you want to shut down the bot?");

            var cancellationtokensource = context.Services.GetService<SharedData>().CancellationTokenSource;
            var interactivity = context.Services.GetService<InteractivityExtension>();
            var message = await interactivity.WaitForMessageAsync(x => x.ChannelId == context.Channel.Id && x.Author.Id == context.Member.Id, TimeSpan.FromSeconds(30));

            if (message.TimedOut)
            {
                await context.SafeRespondUnformattedAsync("⚠️⌛ Timed out.");
            }
            else if (InteractivityUtil.Confirm(message.Result))
            {
                await context.SafeRespondUnformattedAsync("✅ Shutting down.");
                cancellationtokensource.Cancel(false);
            }
            else
            {
                await context.SafeRespondUnformattedAsync("✅ Operation cancelled by user.");
            }
        }

        [Command("grantxp"), Aliases("gxp"), Hidden, RequireOwner]
        public async Task GrantXpAsync(CommandContext context, DiscordMember member, int experience)
        {
            using (var db = Database.CreateContext())
            {
                var data = db.Levels.FirstOrDefault(x => x.UserId == (long)member.Id && x.GuildId == (long)context.Guild.Id);

                if (data != null)
                {
                    data.Experience += experience;
                    await context.RespondAsync($"✅ Granted {experience} xp to {member.DisplayName}.");
                    db.Levels.Update(data);

                    await db.SaveChangesAsync();
                }
                else
                {
                    await context.RespondAsync("⚠️ No xp data stored for this user/guild combo");
                    return;
                }
            }
        }

        [Command("nukeban"), RequireOwner]
        [Description("Bans a member from all servers you own that have ModCore")]
        public async Task NukeBanAsync(CommandContext context, ulong userId, string reason = "")
        {
            await context.RespondAsync($"‼️ This will ban the user with ID {userId} from all servers you own. Proceed?" +
                $"\n**Be wary that this will ACTUALLY ban them from all servers you own, whether they are part of this server or not.**");
            var response = await context.Message.GetNextMessageAsync();
            if (!response.TimedOut && (response.Result?.Content.ToLower() == "yes" || response.Result?.Content.ToLower() == "y"))
            {
                int skip = 0;
                var servers = this.Shared.ModCore.Shards.SelectMany(x => x.Client.Guilds.Values).Where(x => x.Owner.Id == context.Member.Id);
                foreach (var server in servers)
                {
                    if (server.CurrentMember.Roles.Any(x => x.CheckPermission(Permissions.BanMembers) == PermissionLevel.Allowed))
                    {
                        await server.BanMemberAsync(userId, 0, $"[ModCore NukeBan] {reason}");
                    }
                    else
                    {
                        skip++;
                    }
                }
                await context.RespondAsync($"🚓 Succesfully nukebanned member from {servers.Count()} servers." +
                    $"{(skip > 0 ? $" Skipped {skip} servers due to lacking permissions" : "")}");
            }
            else
            {
                await context.RespondAsync("Action canceled.");
            }
        }
    }
}
