using DSharpPlus.EventArgs;
using ModCore.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Listeners
{
    public static class MessageSnipe
    {
        [AsyncListener(EventTypes.MessageDeleted)]
        public static async Task MessageSniped(ModCoreShard bot, MessageDeleteEventArgs e)
        {
            if(!string.IsNullOrEmpty(e.Message.Content) || e.Message.Embeds.Count > 0)
            {
                if (bot.SharedData.DeletedMessages.ContainsKey(e.Channel.Id))
                {
                    bot.SharedData.DeletedMessages[e.Channel.Id] = e.Message;
                }
                else
                {
                    bot.SharedData.DeletedMessages.TryAdd(e.Channel.Id, e.Message);
                }
            }
        }
    }
}
