using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronBlock;

namespace ModCore.CustomCommands
{
    public class RespondBlock : IBlock
    {
        public override async Task<object> EvaluateAsync(Context context)
        {
            var text = this.Fields.First(x => x.Name == "MESSAGE");
            var mc = (DependencyBlockly)context.Dependency;

            await mc.CommandContext.RespondAsync(text.Value);
            return base.EvaluateAsync(context);
        }
    }
}
