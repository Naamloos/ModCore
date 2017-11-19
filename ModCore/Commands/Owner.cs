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

namespace ModCore.Commands
{
    [Group("owner"), Aliases("o"), RequireOwner]
    public class Owner
    {
        public SharedData Shared { get; }

        public Owner(SharedData shared)
        {
            this.Shared = shared;
        }

        [Command("exit"), Aliases("e")]
        public async Task ExitAsync(CommandContext ctx)
        {
            await ctx.RespondAsync("Are you sure you want to shut down the bot?");

            var cts = ctx.Services.GetService<SharedData>().CTS;
            var interactivity = ctx.Services.GetService<InteractivityExtension>();
            var m = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Member.Id, TimeSpan.FromSeconds(30));

            if (m == null)
                await ctx.RespondAsync("Timed out.");
            else if (m.Message.Content == "yes")
            {
                await ctx.RespondAsync("Shutting down.");
                cts.Cancel(false);
            }
            else
                await ctx.RespondAsync("Operation canceled by user.");
        }

        [Command("testupdate"), Aliases("t"), Hidden]
        public async Task ThrowAsync(CommandContext ctx)
        {
            await ctx.RespondAsync("Test: 420");
        }

        [Command("update"), Aliases("u")]
        public async Task UpdateAsync(CommandContext ctx)
        {
            var m = await ctx.RespondAsync($"Running update script...");
            string file = "update";
            if (File.Exists("update.sh"))
            {
                file += ".sh";
                Process proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "nohup",
                        Arguments = "bash update.sh " + Process.GetCurrentProcess().Id,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
            }
            else if (File.Exists("update.bat"))
            {
                file += ".bat";
                Process proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = file,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
            }
            else
            {
                await m.ModifyAsync("**‼ Your update script has not been found. ‼**\n\nPlease place `update.sh` (Linux) or `update.bat` (Windows) in your ModCore directory.");
                return;
            }
            await m.ModifyAsync($"Updating ModCore using `{file}`. See you soon!");
            this.Shared.CTS.Cancel();
        }
    }
}
