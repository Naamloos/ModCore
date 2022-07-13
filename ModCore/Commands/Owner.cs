using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;
using ModCore.Logic.Extensions;

namespace ModCore.Commands
{
    [Group("owner"), Aliases("o"), Hidden]
    public class Owner : BaseCommandModule
	{
        public SharedData Shared { get; }
        public DatabaseContextBuilder Database { get; }

        public Owner(SharedData shared, DatabaseContextBuilder db)
        {
            this.Shared = shared;
            this.Database = db;
        }

        [Command("clear"), Hidden]
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

	    [Command("exit"), Aliases("e"), Hidden]
        public async Task ExitAsync(CommandContext context)
        {
            if (!Shared.BotManagers.Contains(context.Member.Id) && !context.Client.CurrentApplication.Owners.Any(x => x.Id == context.User.Id))
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

        [Command("sudo"), Aliases("s"), Hidden]
        public async Task SudoAsync(CommandContext context, [Description("Member to sudo")]DiscordMember member, [Description("Command to sudo"), RemainingText]string command)
        {
            if (!Shared.BotManagers.Contains(context.Member.Id) && !context.Client.CurrentApplication.Owners.Any(x => x.Id == context.User.Id))
            {
                await context.SafeRespondUnformattedAsync("⚠️ You do not have permission to use this command!");
                return;
            }

            var commandobject = context.CommandsNext.FindCommand(command, out string args);
            var prefix = context.GetGuildSettings()?.Prefix ?? this.Shared.DefaultPrefix;
            var fakecontext = context.CommandsNext.CreateFakeContext(member, context.Channel, command, prefix, commandobject, args);
            await context.CommandsNext.ExecuteCommandAsync(fakecontext);
        }

        [Command("sudoowner"), Aliases("so"), Hidden]
        public async Task SudoOwnerAsync(CommandContext context, [RemainingText, Description("Command to sudo")]string command)
        {
            if (!Shared.BotManagers.Contains(context.Member.Id) && !context.Client.CurrentApplication.Owners.Any(x => x.Id == context.User.Id))
            {
                await context.SafeRespondUnformattedAsync("⚠️ You do not have permission to use this command!");
                return;
            }

            var commandobject = context.CommandsNext.FindCommand(command, out string args);
            var prefix = context.GetGuildSettings()?.Prefix ?? this.Shared.DefaultPrefix;
            var fakecontext = context.CommandsNext.CreateFakeContext(context.Guild.Owner, context.Channel, command, prefix, commandobject, args);
            await context.CommandsNext.ExecuteCommandAsync(fakecontext);
        }

        [Command("grantxp"), Aliases("gxp"), Hidden]
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
	}
}
