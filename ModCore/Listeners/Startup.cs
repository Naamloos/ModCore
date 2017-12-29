using DSharpPlus.EventArgs;
using ModCore.Entities;
using ModCore.Logic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Listeners
{
    public static class Startup
    {
        [AsyncListener(EventTypes.GuildAvailable)]
        public static async Task GuildAvailable(ModCoreShard bot, GuildCreateEventArgs e)
        {
            try
            {
                if (ulong.Parse(ModCore.Args[1]) == e.Guild.Id)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        dynamic obj = JObject.Parse(await client.GetStringAsync("https://icanhazdadjoke.com/slack"));
                        await e.Guild.GetChannel(ulong.Parse(ModCore.Args[2])).SendMessageAsync($"Updated!\n```{obj.text}```");
                    }
                    
                }
            }
            catch
            {
                // TODO: Make SSG Proud again
            }
        }
    }
}
