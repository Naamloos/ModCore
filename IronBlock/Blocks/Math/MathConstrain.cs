using System;
using System.Threading.Tasks;
using IronBlock.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Math
{
    public class MathConstrain : IBlock
    {
        public override async Task<object> EvaluateAsync(Context context)
        {
            var value = (double) await this.Values.EvaluateAsync("VALUE", context);
            var low = (double) await this.Values.EvaluateAsync("LOW", context);
            var high = (double) await this.Values.EvaluateAsync("HIGH", context);

            return System.Math.Min(System.Math.Max(value, low), high);
        }

		public override SyntaxNode Generate(Context context)
		{
			var valueExpression = this.Values.Generate("VALUE", context) as ExpressionSyntax;
			if (valueExpression == null) throw new ApplicationException($"Unknown expression for value.");

			var lowExpression = this.Values.Generate("LOW", context) as ExpressionSyntax;
			if (lowExpression == null) throw new ApplicationException($"Unknown expression for low.");

			var highExpression = this.Values.Generate("HIGH", context) as ExpressionSyntax;
			if (highExpression == null) throw new ApplicationException($"Unknown expression for high.");

			return
				SyntaxGenerator.MethodInvokeExpression(
					IdentifierName(nameof(System.Math)), 
					nameof(System.Math.Min),
					new[]
					{
						SyntaxGenerator.MethodInvokeExpression(
							IdentifierName(nameof(System.Math)),
							nameof(System.Math.Max),
							new [] { valueExpression, lowExpression }
						),
						highExpression
					}
				);
		}
	}

}