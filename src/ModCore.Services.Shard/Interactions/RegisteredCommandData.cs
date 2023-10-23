using ModCore.Services.Shard.Interactions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Services.Shard.Interactions
{
    public struct RegisteredCommandData
    {
        public Type ContainerType;
        public MethodInfo MethodInfo;
        public CommandAttribute Command;
    }
}
