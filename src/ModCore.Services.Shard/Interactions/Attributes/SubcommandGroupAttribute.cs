using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Services.Shard.Interactions.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SubcommandGroupAttribute : CommandAttribute
    {
        public SubcommandGroupAttribute(string name, string description, bool allow_dm = false, bool nsfw = false) : base(name, description, allow_dm, nsfw)
        {
        }
    }
}
