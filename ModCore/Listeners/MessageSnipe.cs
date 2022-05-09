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
        public static async Task MessageSniped(ModCoreShard bot, MessageDeleteEventArgs eventargs)
        {
            await Task.Yield();

            if((!string.IsNullOrEmpty(eventargs.Message.Content) || eventargs.Message.Embeds.Count > 0) && !eventargs.Message.Author.IsBot)
            {
                if (bot.SharedData.DeletedMessages.ContainsKey(eventargs.Channel.Id))
                {
                    bot.SharedData.DeletedMessages[eventargs.Channel.Id] = eventargs.Message;
                }
                else
                {
                    bot.SharedData.DeletedMessages.TryAdd(eventargs.Channel.Id, eventargs.Message);
                }
            }
        }

        [AsyncListener(EventTypes.MessageUpdated)]
        public static async Task MessageEdited(ModCoreShard bot, MessageUpdateEventArgs eventargs)
        {
            await Task.Yield();

            if ((!string.IsNullOrEmpty(eventargs.MessageBefore.Content) || eventargs.Message.Embeds.Count > 0) && !eventargs.Message.Author.IsBot)
            {
                if (bot.SharedData.EditedMessages.ContainsKey(eventargs.Channel.Id))
                {
                    bot.SharedData.EditedMessages[eventargs.Channel.Id] = eventargs.MessageBefore;
                }
                else
                {
                    bot.SharedData.EditedMessages.TryAdd(eventargs.Channel.Id, eventargs.MessageBefore);
                }
            }
        }
    }
}
