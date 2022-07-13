using System;

namespace ModCore.Extensions.Modals.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ModalHiddenFieldAttribute : Attribute
    {
        public string FieldName { get; private set; }

        public ModalHiddenFieldAttribute(string fieldName)
        {
            FieldName = fieldName;
        }
    }
}
