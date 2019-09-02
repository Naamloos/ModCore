using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Controls
{
	public class ControlsFor : IBlock
    {
        public override async Task<object> EvaluateAsync(Context context)
        {
            var variableName = this.Fields.Get("VAR");
            
            var fromValue = (double) await this.Values.EvaluateAsync("FROM", context);
            var toValue = (double) await this.Values.EvaluateAsync("TO", context);
            var byValue = (double) await this.Values.EvaluateAsync("BY", context);
            
            var statement = this.Statements.FirstOrDefault();


            if (context.Variables.ContainsKey(variableName))
            {
                context.Variables[variableName] = fromValue;
            }
            else
            {
                context.Variables.Add(variableName, fromValue);
            }


            while ((double) context.Variables[variableName] <= toValue)
            {
                await statement.EvaluateAsync(context);
                context.Variables[variableName] = (double) context.Variables[variableName] + byValue;
            }

            return await base.EvaluateAsync(context);
        }

		public override SyntaxNode Generate(Context context)
		{
			var variableName = this.Fields.Get("VAR").CreateValidName();

			var fromValueExpression = this.Values.Generate("FROM", context) as ExpressionSyntax;
			if (fromValueExpression == null) throw new ApplicationException($"Unknown expression for from value.");

			var toValueExpression = this.Values.Generate("TO", context) as ExpressionSyntax;
			if (toValueExpression == null) throw new ApplicationException($"Unknown expression for to value.");

			var byValueExpression = this.Values.Generate("BY", context) as ExpressionSyntax;
			if (byValueExpression == null) throw new ApplicationException($"Unknown expression for by value.");

			var statement = this.Statements.FirstOrDefault();

			var rootContext = context.GetRootContext();
			if (!rootContext.Variables.ContainsKey(variableName))
			{
				rootContext.Variables[variableName] = null;
			}

			var forContext = new Context(context.Dependency) { Parent = context };
			if (statement?.Block != null)
			{
				var statementSyntax = statement.Block.GenerateStatement(forContext);
				if (statementSyntax != null)
				{
					forContext.Statements.Add(statementSyntax);
				}
			}

			var forStatement =
					ForStatement(
								Block(forContext.Statements)
							)
							.WithInitializers(
								SingletonSeparatedList<ExpressionSyntax>(
									AssignmentExpression(
										SyntaxKind.SimpleAssignmentExpression,
										IdentifierName(variableName),
										fromValueExpression
									)
								)
							)
							.WithCondition(
								BinaryExpression(
									SyntaxKind.LessThanOrEqualExpression,
									IdentifierName(variableName),
									toValueExpression
								)
							)
							.WithIncrementors(
								SingletonSeparatedList<ExpressionSyntax>(
									AssignmentExpression(
										SyntaxKind.AddAssignmentExpression,
										IdentifierName(variableName),
										byValueExpression
									)
								)
							);

			return Statement(forStatement, base.Generate(context), context);
		}
	}

}