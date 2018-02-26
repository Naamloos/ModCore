using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;

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

        [Command("exit"), Aliases("e"), Hidden]
        public async Task ExitAsync(CommandContext ctx)
        {
            if (!Shared.BotManagers.Contains(ctx.Member.Id) && ctx.Client.CurrentApplication.Owner != ctx.User)
            {
                await ctx.SafeRespondAsync("You do not have permission to use this command!");
                return;
            }

            await ctx.SafeRespondAsync("Are you sure you want to shut down the bot?");

            var cts = ctx.Services.GetService<SharedData>().CTS;
            var interactivity = ctx.Services.GetService<InteractivityExtension>();
            var m = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Member.Id, TimeSpan.FromSeconds(30));

            if (m == null)
            {
                await ctx.SafeRespondAsync("Timed out.");
            }
            else if (InteractivityUtil.Confirm(m))
            {
                await ctx.SafeRespondAsync("Shutting down.");
                cts.Cancel(false);
            }
            else
            {
                await ctx.SafeRespondAsync("Operation cancelled by user.");
            }
        }

        [Command("testupdate"), Aliases("t"), Hidden]
        public async Task ThrowAsync(CommandContext ctx)
        {
            if (!Shared.BotManagers.Contains(ctx.Member.Id) && ctx.Client.CurrentApplication.Owner != ctx.User)
            {
                await ctx.SafeRespondAsync("You do not have permission to use this command!");
                return;
            }
            await ctx.SafeRespondAsync("Test: 420");
        }

        [Command("sudo"), Aliases("s"), Hidden]
        public async Task SudoAsync(CommandContext ctx, [Description("Member to sudo")]DiscordMember m, [Description("Command to sudo"), RemainingText]string command)
        {
            if (!Shared.BotManagers.Contains(ctx.Member.Id) && ctx.Client.CurrentApplication.Owner != ctx.User)
            {
                await ctx.SafeRespondAsync("You do not have permission to use this command!");
                return;
            }
            await ctx.CommandsNext.SudoAsync(m, ctx.Channel, command);
        }

        [Command("sudoowner"), Aliases("so"), Hidden]
        public async Task SudoOwnerAsync(CommandContext ctx, [RemainingText, Description("Command to sudo")]string command)
        {
            if (!Shared.BotManagers.Contains(ctx.Member.Id) && ctx.Client.CurrentApplication.Owner != ctx.User)
            {
                await ctx.SafeRespondAsync("You do not have permission to use this command!");
                return;
            }
            await ctx.CommandsNext.SudoAsync(ctx.Guild.Owner, ctx.Channel, command);
        }

        [Command("update"), Aliases("u"), Hidden]
        public async Task UpdateAsync(CommandContext ctx)
        {
            if (!Shared.BotManagers.Contains(ctx.Member.Id) && ctx.Client.CurrentApplication.Owner != ctx.User)
            {
                await ctx.SafeRespondAsync("You do not have permission to use this command!");
                return;
            }
            var m = await ctx.SafeRespondAsync($"Running update script...");
            const string fn = "update";
            if (File.Exists("update.sh"))
            {
                const string file = fn + ".sh";
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "nohup",
                        Arguments = $"bash {file} {Process.GetCurrentProcess().Id} {ctx.Guild.Id} {ctx.Channel.Id}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                await m.ModifyAsync($"Updating ModCore using `{file}`. See you soon!");
            }
            else if (File.Exists("update.bat"))
            {
                const string file = fn + ".bat";
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = file,
                        Arguments = $"{ctx.Guild.Id} {ctx.Channel.Id}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                await m.ModifyAsync($"Updating ModCore using `{file}`. See you soon!");
            }
            else
            {
                await m.ModifyAsync("**‼ Your update script has not been found. ‼**\n\nPlease place `update.sh` (Linux) or `update.bat` (Windows) in your ModCore directory.");
                return;
            }
            this.Shared.CTS.Cancel();
        }

        [Command("botmanagers"), Aliases("bm"), Hidden]
        public async Task BotManagersAsync(CommandContext ctx)
        {
            if (!Shared.BotManagers.Contains(ctx.Member.Id) && ctx.Client.CurrentApplication.Owner != ctx.User)
            {
                await ctx.SafeRespondAsync("You do not have permission to use this command!");
                return;
            }
            var list = new List<string>();
            foreach (ulong manager in Shared.BotManagers)
            {
                try
                {
                    DiscordMember m = await ctx.Guild.GetMemberAsync(manager);
                    list.Add(m.DisplayName);
                }
                catch
                {

                }
            }
            await ctx.SafeRespondAsync($"Users with access: {(list.Count > 0 ? $"`{string.Join("`, `", list)}`" : "None")}");
        }
    }
}
