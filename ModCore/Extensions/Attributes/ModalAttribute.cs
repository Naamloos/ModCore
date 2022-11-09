using System;

namespace ModCore.Extensions.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ModalAttribute : Attribute
    {
        public string ModalId { get; set; }

        public ModalAttribute(string modalId)
        {
            ModalId = modalId;
        }
    }
}
