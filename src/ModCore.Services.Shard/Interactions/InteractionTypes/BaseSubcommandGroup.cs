using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Services.Shard.Interactions.InteractionTypes
{
    public abstract class BaseSubcommandGroup<TParent>
    {
        protected TParent Parent { get; set; }

        public BaseSubcommandGroup(TParent parent)
        {
            Parent = parent;
        }
    }
}
