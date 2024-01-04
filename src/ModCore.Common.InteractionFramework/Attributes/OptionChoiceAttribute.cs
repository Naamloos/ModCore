using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.InteractionFramework.Attributes
{
    public class OptionChoiceAttribute
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public OptionChoiceAttribute(string name, string value) 
        { 
            this.Name = name;
            this.Value = value;
        }

        public OptionChoiceAttribute(string name, double value)
        {
            this.Name = name;
            this.Value = value;
        }

        public OptionChoiceAttribute(string name, long value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
