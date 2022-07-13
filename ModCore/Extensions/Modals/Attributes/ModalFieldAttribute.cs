using System;
using DSharpPlus;

namespace ModCore.Extensions.Modals.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ModalFieldAttribute : Attribute
    {
        public string DisplayText { get; set; }
        public string FieldName { get; set; }
        public string Placeholder { get; set; }
        public string Prefill { get; set; }
        public bool Required { get; set; }
        public TextInputStyle InputStyle { get; set; }
        public int MinLength { get; set; }
        public int? MaxLength { get; set; }

        public ModalFieldAttribute(string displaytext, string fieldName, string placeholder = null,
            string prefill = null, bool required = false, TextInputStyle style = TextInputStyle.Short,
            int min_length = 0, int max_length = -1)
        {
            DisplayText = displaytext;
            FieldName = fieldName;
            Placeholder = placeholder;
            Required = required;
            Prefill = prefill;
            InputStyle = style;
            MinLength = min_length;
            MaxLength = max_length < 0 ? null : max_length;
        }
    }
}
