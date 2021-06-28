using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
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
        public async Task ExecuteAsync(CommandContext ctx)
        {
            await ctx.RespondAsync($"ModCore BETA (rewrite)");
        }
    }
}
