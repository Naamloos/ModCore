using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Modules
{
    public class GeneralModule : BaseCommandModule
    {
        public GeneralModule()
        {
        }

        [Command("about")]
        public async Task AboutAsync(CommandContext ctx)
        {
            await ctx.RespondAsync($"ModCore BETA (rewrite)");
        }

        [Command("testStorage")]
        public async Task TestStorage(CommandContext ctx)
        {
            var b = new StorageBuilder().ForGuild(ctx.Guild.Id).ForUser(ctx.User.Id);
            var s = b.Build();

            await ctx.RespondAsync(s.GetPath());
        }
    }
}
