using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using ModCore.Entities;

namespace ModCore.Commands
{
    [Group("owner"), Aliases("o"), RequireOwner]
    public class Owner
    {
        [Command("exit"), Aliases("e")]
        public async Task ExitAsync(CommandContext ctx)
        {
            await ctx.RespondAsync("Are you sure you want to shut down the bot?");

            var cts = ctx.Dependencies.GetDependency<SharedData>().CTS;
            var interactivity = ctx.Dependencies.GetDependency<InteractivityExtension>();
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
    }
}
