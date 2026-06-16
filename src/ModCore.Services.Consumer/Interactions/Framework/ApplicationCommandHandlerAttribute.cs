using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Services.Consumer.Interactions.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ApplicationCommandHandlerAttribute : Attribute
    {
    }
}
