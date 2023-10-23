using ModCore.Common.Discord.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Services.Shard.Interactions.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CommandAttribute : Attribute
    {
        public readonly string Name;
        public readonly string Description;
        public readonly bool AllowDM;
        public readonly bool NSFW;

        public CommandAttribute(string name, string description, bool allow_dm = false, bool nsfw = false)
        {
            this.Name = name;
            this.Description = description;
            this.AllowDM = allow_dm;
            this.NSFW = nsfw;
        }
    }
}
