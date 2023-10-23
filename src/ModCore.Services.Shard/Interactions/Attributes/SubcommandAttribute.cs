using ModCore.Common.Discord.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Services.Shard.Interactions.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SubcommandAttribute : CommandAttribute
    {
        public SubcommandAttribute(string name, string description, bool allow_dm = false, bool nsfw = false) : base(name, description, allow_dm, nsfw)
        {
        }
    }
}
