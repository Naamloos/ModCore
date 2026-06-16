using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Services.Consumer.Handlers
{
    public abstract class BaseHandler { }

    public abstract class BaseHandler<T> : BaseHandler
    {
        public abstract Task HandleAsync(T payload, CancellationToken cancellationToken);
    }
}
