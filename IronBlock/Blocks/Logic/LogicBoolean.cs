using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Logic
{
    public class LogicBoolean : IBlock
    {
        public override async Task<object> EvaluateAsync(Context context)
        {
            return bool.Parse(this.Fields.Get("BOOL"));
        }

		public override SyntaxNode Generate(Context context)
		{
			bool value = bool.Parse(this.Fields.Get("BOOL"));
			if (value)
				return LiteralExpression(SyntaxKind.TrueLiteralExpression);

			return LiteralExpression(SyntaxKind.FalseLiteralExpression);
		}
	}
}