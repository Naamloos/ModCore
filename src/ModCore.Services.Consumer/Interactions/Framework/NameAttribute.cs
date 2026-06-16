using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Services.Consumer.Interactions.Framework
{
    public sealed class NameAttribute : Attribute
    {
        public string Name { get; private set; }

        public NameAttribute(string name)
        {
            Name = name;
        }
    }
}
