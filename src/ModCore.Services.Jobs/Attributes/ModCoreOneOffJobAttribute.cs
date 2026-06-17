using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Services.Jobs.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModCoreOneOffJobAttribute : Attribute
    {
        public string Key { get; private set; }

        public ModCoreOneOffJobAttribute(string key)
        {
            Key = key;
        }
    }
}
