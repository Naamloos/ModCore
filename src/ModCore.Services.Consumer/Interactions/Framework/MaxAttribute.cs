using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Services.Consumer.Interactions.Framework
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class MaxAttribute : Attribute
    {
        public int Max { get; private set; }

        public MaxAttribute(int max)
        {
            Max = max;
        }
    }
}
