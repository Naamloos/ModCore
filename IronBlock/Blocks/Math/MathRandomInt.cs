using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronBlock.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Math
{
	public class MathRandomInt : IBlock
	{
		static Random rand = new Random();

        public override async Task<object> EvaluateAsync(Context context)
		{
			var from = (double) await this.Values.EvaluateAsync("FROM", context);
			var to = (double) await this.Values.EvaluateAsync("TO", context);
			return (double) rand.Next((int)System.Math.Min(from, to), (int)System.Math.Max(from, to));
		}

	}
}