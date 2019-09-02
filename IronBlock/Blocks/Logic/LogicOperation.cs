using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Logic
{
    public class LogicOperation : IBlock
    {
        public override async Task<object> EvaluateAsync(Context context)
        {
            var a = (bool) (await this.Values.EvaluateAsync("A", context) ?? false);
            var b = (bool) (await this.Values.EvaluateAsync("B", context) ?? false);
            
            var op = this.Fields.Get("OP");

            switch (op)
            {
                case "AND": return a && b;
                case "OR": return a || b;
                default: throw new ApplicationException($"Unknown OP {op}");
            }

        }

		public override SyntaxNode Generate(Context context)
		{
			var firstExpression = this.Values.Generate("A", context) as ExpressionSyntax;
			if (firstExpression == null) throw new ApplicationException($"Unknown expression for value A.");

			var secondExpression = this.Values.Generate("B", context) as ExpressionSyntax;
			if (secondExpression == null) throw new ApplicationException($"Unknown expression for value B.");

			var opValue = this.Fields.Get("OP");

			var binaryOperator = GetBinaryOperator(opValue);
			var expression = BinaryExpression(binaryOperator, firstExpression, secondExpression);

			return ParenthesizedExpression(expression);
		}

		private SyntaxKind GetBinaryOperator(string opValue)
		{
			switch (opValue)
			{
				case "AND": return SyntaxKind.LogicalAndExpression;
				case "OR": return SyntaxKind.LogicalOrExpression;

				default: throw new ApplicationException($"Unknown OP {opValue}");
			}
		}
	}

}