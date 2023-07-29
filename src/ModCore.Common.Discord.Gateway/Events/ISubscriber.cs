using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Gateway.Events
{
    public interface ISubscriber<T> : ISubscriber where T : IPublishable
    {
        public Task HandleEvent(T data);
    }

    public interface ISubscriber { }
}
