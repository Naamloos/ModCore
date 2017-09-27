using System;
using System.Threading.Tasks;
using ModCore.Logic;

namespace ModCore.Listeners
{
    public class ReadyListeners
    {
        [AsyncListener(EventTypes.SocketOpened)]
        public static async Task OnSocketOpened(Bot bot)
        {
            await Task.Yield();
            bot.SocketStart = DateTimeOffset.Now;
        }
    }
}