using IronBlock.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Threading.Tasks;

namespace IronBlock.Blocks.Text
{
	public class TextTrim : IBlock
    {
        public override async Task<object> EvaluateAsync(Context context)
        {
            var mode = this.Fields.Get("MODE");

            var text = (await this.Values.EvaluateAsync("TEXT", context) ?? "").ToString();

            switch (mode)
            {
                case "BOTH": return text.Trim();
                case "LEFT": return text.TrimStart();
                case "RIGHT": return text.TrimEnd();
                default: throw new ApplicationException("unknown mode");
            }
        }

		public override SyntaxNode Generate(Context context)
		{
			var textExpression = this.Values.Generate("TEXT", context) as ExpressionSyntax;
			if (textExpression == null) throw new ApplicationException($"Unknown expression for text.");

			var mode = this.Fields.Get("MODE");

			switch (mode)
			{
				case "BOTH": return SyntaxGenerator.MethodInvokeExpression(textExpression, nameof(string.Trim));
				case "LEFT": return SyntaxGenerator.MethodInvokeExpression(textExpression, nameof(string.TrimStart));
				case "RIGHT": return SyntaxGenerator.MethodInvokeExpression(textExpression, nameof(string.TrimEnd));
				default: throw new ApplicationException("unknown mode");
			}
		}
	}
}