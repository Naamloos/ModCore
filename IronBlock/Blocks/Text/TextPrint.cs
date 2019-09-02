using System;
using System.Threading.Tasks;
using IronBlock.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Text
{
    public class TextPrint : IBlock
    {
        public override async Task<object> EvaluateAsync(Context context)
        {
            var text = await this.Values.EvaluateAsync("TEXT", context);

            Console.WriteLine(text);

            return await base.EvaluateAsync(context);
        }

		public override SyntaxNode Generate(Context context)
		{
			SyntaxNode syntaxNode = this.Values.Generate("TEXT", context);
			var expression = syntaxNode as ExpressionSyntax;
			if (expression == null) throw new ApplicationException($"Unknown expression for text.");

			var invocationExpression =
				SyntaxGenerator.MethodInvokeExpression(IdentifierName(nameof(Console)), nameof(Console.WriteLine), expression);
			
			return Statement(invocationExpression, base.Generate(context), context);
		}
	}

}