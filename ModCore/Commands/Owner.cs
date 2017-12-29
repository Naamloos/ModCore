using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using ModCore.Entities;
using System.IO;
using System.Diagnostics;
using DSharpPlus.Entities;
using ModCore.Logic;
using ModCore.Database;
using System.Collections.Generic;

namespace ModCore.Commands
{
    [Group("owner"), Aliases("o")]
    public class Owner
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
                await ctx.RespondAsync("You do not have permission to use this command!");
                return;
            }

            await ctx.RespondAsync("Are you sure you want to shut down the bot?");

            var cts = ctx.Services.GetService<SharedData>().CTS;
            var interactivity = ctx.Services.GetService<InteractivityExtension>();
            var m = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Member.Id, TimeSpan.FromSeconds(30));

            if (m == null)
            {
                await ctx.RespondAsync("Timed out.");
            }
            else if (InteractivityUtil.Confirm(m))
            {
                await ctx.RespondAsync("Shutting down.");
                cts.Cancel(false);
            }
            else
            {
                await ctx.RespondAsync("Operation cancelled by user.");
            }
        }

        [Command("testupdate"), Aliases("t"), Hidden]
        public async Task ThrowAsync(CommandContext ctx)
        {
            if (!Shared.BotManagers.Contains(ctx.Member.Id) && ctx.Client.CurrentApplication.Owner != ctx.User)
            {
                await ctx.RespondAsync("You do not have permission to use this command!");
                return;
            }
            await ctx.RespondAsync("Test: 420");
        }

        [Command("sudo"), Aliases("s"), Hidden]
        public async Task SudoAsync(CommandContext ctx, [Description("Member to sudo")]DiscordMember m, [Description("Command to sudo"), RemainingText]string command)
        {
            if (!Shared.BotManagers.Contains(ctx.Member.Id) && ctx.Client.CurrentApplication.Owner != ctx.User)
            {
                await ctx.RespondAsync("You do not have permission to use this command!");
                return;
            }
            await ctx.CommandsNext.SudoAsync(m, ctx.Channel, command);
        }

        [Command("sudoowner"), Aliases("so"), Hidden]
        public async Task SudoOwnerAsync(CommandContext ctx, [RemainingText, Description("Command to sudo")]string command)
        {
            if (!Shared.BotManagers.Contains(ctx.Member.Id) && ctx.Client.CurrentApplication.Owner != ctx.User)
            {
                await ctx.RespondAsync("You do not have permission to use this command!");
                return;
            }
            await ctx.CommandsNext.SudoAsync(ctx.Guild.Owner, ctx.Channel, command);
        }

        [Command("update"), Aliases("u"), Hidden]
        public async Task UpdateAsync(CommandContext ctx)
        {
            if (!Shared.BotManagers.Contains(ctx.Member.Id) && ctx.Client.CurrentApplication.Owner != ctx.User)
            {
                await ctx.RespondAsync("You do not have permission to use this command!");
                return;
            }
            var m = await ctx.RespondAsync($"Running update script...");
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

        [Command("givebotmanager"), Aliases("gbm"), Hidden]
        public async Task GiveBotManagerAsync(CommandContext ctx, DiscordMember m)
        {
            if (!Shared.BotManagers.Contains(ctx.Member.Id) && ctx.Client.CurrentApplication.Owner != ctx.User)
            {
                await ctx.RespondAsync("You do not have permission to use this command!");
                return;
            }
            if (Shared.BotManagers.Contains(m.Id))
            {
                await ctx.RespondAsync("That person is already in the database.");
                return;
            }
            Shared.BotManagers.Add(m.Id);
        }

        [Command("takebotmanager"), Aliases("tbm"), Hidden]
        public async Task TakeBotManagerAsync(CommandContext ctx, DiscordMember m)
        {
            if (!Shared.BotManagers.Contains(ctx.Member.Id) && ctx.Client.CurrentApplication.Owner != ctx.User)
            {
                await ctx.RespondAsync("You do not have permission to use this command!");
                return;
            }
            if (!Shared.BotManagers.Contains(m.Id))
            {
                await ctx.RespondAsync("That person is not in the database.");
                return;
            }
            Shared.BotManagers.Remove(m.Id);
        }

        [Command("listbotmanager"), Aliases("lbm"), Hidden]
        public async Task ListBotManagerAsync(CommandContext ctx)
        {
            if (!Shared.BotManagers.Contains(ctx.Member.Id) && ctx.Client.CurrentApplication.Owner != ctx.User)
            {
                await ctx.RespondAsync("You do not have permission to use this command!");
                return;
            }
            var list = new List<string>();
            foreach (ulong manager in Shared.BotManagers)
            {
                try
                {
                    DiscordMember m = await ctx.Guild.GetMemberAsync((ulong)manager);
                    list.Add(m.DisplayName);
                }
                catch
                {

                }
            }
            await ctx.RespondAsync("Users with access: " + (list.Count > 0 ? string.Join(", ", list) : "None"));
        }
    }
}
