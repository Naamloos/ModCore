using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IronBlock.Blocks.Text
{
    public class ColourPicker : IBlock
    {
        public override async Task<object> EvaluateAsync(Context context)
        {
            return this.Fields.Get("COLOUR") ?? "#000000";
        }
    }

}