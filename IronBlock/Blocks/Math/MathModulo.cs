using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Math
{
    public class MathModulo: IBlock
    {
        public override async Task<object> EvaluateAsync(Context context)
        {
            var dividend = (double) await this.Values.EvaluateAsync("DIVIDEND", context);
            var divisor = (double) await this.Values.EvaluateAsync("DIVISOR", context);

            return dividend % divisor;
        }

		public override SyntaxNode Generate(Context context)
		{
			var dividendExpression = this.Values.Generate("DIVIDEND", context) as ExpressionSyntax;
			if (dividendExpression == null) throw new ApplicationException($"Unknown expression for dividend.");

			var divisorExpression = this.Values.Generate("DIVISOR", context) as ExpressionSyntax;
			if (divisorExpression == null) throw new ApplicationException($"Unknown expression for divisor.");

			return BinaryExpression(
				SyntaxKind.ModuloExpression,
				dividendExpression,
				divisorExpression
			);
		}
	}
}