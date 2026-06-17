using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Services.Consumer.Interactions.Framework
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class MinAttribute : Attribute
    {
        public int Min { get; set; }

        public MinAttribute(int min)
        {
            Min = min;
        }
    }
}
