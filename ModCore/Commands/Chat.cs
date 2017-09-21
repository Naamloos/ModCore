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
    [Group("chat"), Aliases("c")]
    public class Chat
    {
        [Command("purge"), Aliases("p")]
        public async Task PurgeAsync(CommandContext ctx, DiscordUser User)
        {
            var ms = await ctx.Channel.GetMessagesAsync(100, ctx.Message.Id);
            foreach(var m in ms)
            {
                if(m.Author.Id == User.Id)
                {
                    await m.DeleteAsync();
                }
            }
            await ctx.RespondAsync($"Latest messages by {User.Mention} (ID:{User.Id}) deleted.");
        }

        [Command("")]
        public async Task SomethingAsync(CommandContext ctx)
        {

        }
    }
}
