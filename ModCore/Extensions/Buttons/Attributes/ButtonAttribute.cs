using System;

namespace ModCore.Extensions.Buttons.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ButtonAttribute : Attribute
    {
        public string Id { get; private set; }

        public ButtonAttribute(string id)
        {
            this.Id = id;
        }
    }
}
