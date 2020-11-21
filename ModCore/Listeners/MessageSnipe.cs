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
            await Task.Yield();

            if((!string.IsNullOrEmpty(e.Message.Content) || e.Message.Embeds.Count > 0) && !e.Message.Author.IsBot)
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

        [AsyncListener(EventTypes.MessageUpdated)]
        public static async Task MessageEdited(ModCoreShard bot, MessageUpdateEventArgs e)
        {
            await Task.Yield();

            if (e.MessageBefore is null)
                return;

            if ((!string.IsNullOrEmpty(e.MessageBefore.Content) || e.Message.Embeds.Count > 0) && !e.Message.Author.IsBot)
            {
                if (bot.SharedData.EditedMessages.ContainsKey(e.Channel.Id))
                {
                    bot.SharedData.EditedMessages[e.Channel.Id] = e.MessageBefore;
                }
                else
                {
                    bot.SharedData.EditedMessages.TryAdd(e.Channel.Id, e.MessageBefore);
                }
            }
        }
    }
}
