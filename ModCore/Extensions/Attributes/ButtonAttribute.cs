using System;

namespace ModCore.Extensions.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ButtonAttribute : Attribute
    {
        public string Id { get; private set; }

        public ButtonAttribute(string id)
        {
            Id = id;
        }
    }
}
