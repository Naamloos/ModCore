using IronBlock.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Lists
{
	public class ListsRepeat : IBlock
	{
        public override async Task<object> EvaluateAsync(Context context)
		{
			var item = await this.Values.EvaluateAsync("ITEM", context);
			var num = (double)await this.Values.EvaluateAsync("NUM", context);

			var list = new List<object>();
			for (var i = 0; i < num; i++)
			{
				list.Add(item);

			}
			return list;

		}

		public override SyntaxNode Generate(Context context)
		{
			var itemExpression = this.Values.Generate("ITEM", context) as ExpressionSyntax;
			if (itemExpression == null) throw new ApplicationException($"Unknown expression for item.");

			var numExpression = this.Values.Generate("NUM", context) as ExpressionSyntax;
			if (numExpression == null) throw new ApplicationException($"Unknown expression for number.");

			return SyntaxGenerator.MethodInvokeExpression(
				SyntaxGenerator.MethodInvokeExpression(
					IdentifierName(nameof(Enumerable)),
					nameof(Enumerable.Repeat),
					new[] { itemExpression, numExpression }
				),
				nameof(Enumerable.ToList)
			);
		}
	}
}