using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using ModCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Modules
{
    public class GeneralModule : BaseCommandModule
    {
        private BotMetaService bot;

        public GeneralModule(BotMetaService bot)
        {
            this.bot = bot;
        }

        [Command("about")]
        public async Task AboutAsync(CommandContext ctx)
        {
            await ctx.RespondAsync($"ModCore BETA (rewrite)");
        }

        [Command("uptime")]
        public async Task UptimeAsync(CommandContext ctx)
        {
            var msg = await ctx.RespondAsync(
                $"Bot start: <t:{bot.StartTime.ToUnixTimeSeconds()}:R>" +
                $"\nSocket start: <t:{bot.SocketStartTime.ToUnixTimeSeconds()}:R>");
            ctx.Message.DestroyIn(TimeSpan.FromSeconds(10));
            msg.DestroyIn(TimeSpan.FromSeconds(10));
        }
    }
}
