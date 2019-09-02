using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Logic
{
	public class LogicNegate : IBlock
	{
        public override async Task<object> EvaluateAsync(Context context)
		{
			return !((bool)(await this.Values.EvaluateAsync("BOOL", context) ?? false));
		}

		public override SyntaxNode Generate(Context context)
		{
			var boolExpression = this.Values.Generate("BOOL", context) as ExpressionSyntax;
			if (boolExpression == null) throw new ApplicationException($"Unknown expression for negate.");

			return PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, boolExpression);
		}
	}

}