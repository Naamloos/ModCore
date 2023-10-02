using CommunityToolkit.HighPerformance.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Gateway.Events
{
    internal readonly struct ParallelEventRunner<T> : IRefAction<ISubscriber> where T : IPublishable
    {
        private readonly Gateway Gateway;
        private readonly T Data;

        public ParallelEventRunner(Gateway gateway, T data)
        {
            this.Gateway = gateway;
            this.Data = data;
        }

        public void Invoke(ref ISubscriber item)
        {
            if (item is ISubscriber<T> qualifiedItem)
            {
                _ = Gateway.runEventHandlerAsync(qualifiedItem, Data);
            }
        }
    }
}
