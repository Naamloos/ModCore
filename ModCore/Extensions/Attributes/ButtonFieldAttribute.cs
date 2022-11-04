using System;

namespace ModCore.Extensions.Attributes
{
    public class ButtonFieldAttribute : Attribute
    {
        public string Name { get; private set; }

        public ButtonFieldAttribute(string name)
        {
            Name = name;
        }
    }
}
