using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace ModCore.Commands
{
    public class Main
    {
        [Command("ping"), Aliases("p")]
        public async Task PingAsync(CommandContext ctx)
        {
            await ctx.RespondAsync($"Pong: ({ctx.Client.Ping}) ms.");
        }

        [Command("uptime"), Aliases("u")]
        public async Task UptimeAsync(CommandContext ctx)
        {
            var b = ctx.Dependencies.GetDependency<Bot>();
            var bup = DateTimeOffset.Now.Subtract(b.ProgramStart);
            var sup = DateTimeOffset.Now.Subtract(b.SocketStart);

            // Needs improvement
            await ctx.RespondAsync($"Program uptime: {String.Format(@"{0} days, {1}", bup.ToString(@"dd"), bup.ToString(@"hh\:mm\:ss"))}\n" +
                $"Socket uptime: {String.Format(@"{0} days, {1}", sup.ToString(@"dd"), sup.ToString(@"hh\:mm\:ss"))}");
        }
    }
}
