using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.InteractionFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class OptionAttribute : Attribute
    {
        public string Description { get; set; }
        public ApplicationCommandOptionType Type { get; set; }
        public IEnumerable<ApplicationCommandOptionChoice>? Choices { get; set; }
        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public bool AutoComplete { get; set; }

        public OptionAttribute(string description, ApplicationCommandOptionType type) 
        { 
            Description = description;
            Type = type;
        }
    }
}
