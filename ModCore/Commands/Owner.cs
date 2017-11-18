using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using ModCore.Entities;
using System.IO;
using System.Diagnostics;

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

        [Command("throw"), Aliases("t"), Hidden]
        public async Task ThrowAsync(CommandContext ctx)
        {
            await ctx.RespondAsync("Throwing exception for testing purposes");
            throw new AccessViolationException("Did you just assume my gender?");
        }

        [Command("update"), Aliases("u")]
        public async Task UpdateAsync(CommandContext ctx)
        {
            string file = "update";
            if (File.Exists("update.sh"))
                file += ".sh";
            else if (File.Exists("update.bat"))
                file += ".bat";
            else
            {
                await ctx.RespondAsync("**‼ Your update script has not been found. ‼**\n\nPlease place `update.sh` (Linux) or `update.bat` (Windows) in your ModCore directory.");
                return;
            }

            var m = await ctx.RespondAsync($"Running `{file}`...");
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
            proc.WaitForExit();
            await m.ModifyAsync($"Updated ModCore using `{file}`. Restarting..");
            this.Shared.CTS.Cancel();
        }
    }
}
