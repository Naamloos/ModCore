using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Text
{
    public class TextBlock : IBlock
    {
        public override async Task<object> EvaluateAsync(Context context)
        {
            var text = this.Fields.Get("TEXT");

            return text;
        }

        public override SyntaxNode Generate(Context context)
        {
            var text = this.Fields.Get("TEXT");

			return LiteralExpression(
					SyntaxKind.StringLiteralExpression,
						Literal(text)
					);
		}
    }

}