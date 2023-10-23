using ModCore.Common.Discord.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Services.Shard.Interactions.Attributes
{
    public class ParameterAttribute : Attribute
    {
        public string Description { get; set; }

        public ParameterAttribute(string description) 
        {
            Description = description;
        }
    }
}
