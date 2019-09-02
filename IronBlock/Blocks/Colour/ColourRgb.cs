using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronBlock.Blocks.Text
{
    public class ColourRgb : IBlock
    {
        Random random = new Random();
        public override async Task<object> EvaluateAsync(Context context)
        {
            var red = Convert.ToByte(await this.Values.EvaluateAsync("RED", context));
            var green = Convert.ToByte(await this.Values.EvaluateAsync("GREEN", context));
            var blue = Convert.ToByte(await this.Values.EvaluateAsync("BLUE", context));

            return $"#{red:x2}{green:x2}{blue:x2}";
        }
    }

}