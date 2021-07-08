using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore
{
    public static class Extensions
    {
        public static void DestroyIn(this DiscordMessage msg, TimeSpan destroy_in)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(destroy_in);
                if (await msg.Channel.GetMessageAsync(msg.Id) != null)
                {
                    await msg.DeleteAsync();
                }
            });
        }
    }
}
