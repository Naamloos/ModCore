using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Variables
{
    public class VariablesSet : IBlock
    {
        public override async Task<object> EvaluateAsync(Context context)
        {
            var variables = context.Variables;

            var value = await this.Values.EvaluateAsync("VALUE", context);

            var variableName = this.Fields.Get("VAR");

            if (variables.ContainsKey(variableName))
            {
                variables[variableName] = value;
            }
            else
            {
                variables.Add(variableName, value);
            }

            return await base.EvaluateAsync(context);
        }

		public override SyntaxNode Generate(Context context)
		{
			var variables = context.Variables;

			var variableName = this.Fields.Get("VAR").CreateValidName();

			var valueExpression = this.Values.Generate("VALUE", context) as ExpressionSyntax;
			if (valueExpression == null)
				throw new ApplicationException($"Unknown expression for value.");

			context.GetRootContext().Variables[variableName] = valueExpression;

			var assignment = AssignmentExpression(
								SyntaxKind.SimpleAssignmentExpression,
									IdentifierName(variableName),
									valueExpression
								);					

			return Statement(assignment, base.Generate(context), context);
		}
	}

}