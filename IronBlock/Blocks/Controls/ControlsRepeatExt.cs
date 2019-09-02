using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Controls
{
    public class ControlsRepeatExt : IBlock
    {
        public override async Task<object> EvaluateAsync(Context context)
        {
            var timesValue = (double) await this.Values.EvaluateAsync("TIMES", context);

            if (!this.Statements.Any(x => x.Name == "DO")) return base.EvaluateAsync(context);

            var statement = this.Statements.Get("DO");

            for (var i = 0; i < timesValue; i++)
            {
                await statement.EvaluateAsync(context);

                if (context.EscapeMode == EscapeMode.Break)
                {
                    context.EscapeMode = EscapeMode.None;
                    break;
                }

                context.EscapeMode = EscapeMode.None;
            }

            context.EscapeMode = EscapeMode.None;
                
            return await base.EvaluateAsync(context);
        }
		
		public override SyntaxNode Generate(Context context)
		{
			var timesExpression = this.Values.Generate("TIMES", context) as ExpressionSyntax;
			if (timesExpression == null) throw new ApplicationException($"Unknown expression for times.");

			if (!this.Statements.Any(x => x.Name == "DO")) return base.Generate(context);

			var statement = this.Statements.Get("DO");

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
							.WithDeclaration(
								VariableDeclaration(
									PredefinedType(
										Token(SyntaxKind.IntKeyword)
									)
								)
								.WithVariables(
									SingletonSeparatedList<VariableDeclaratorSyntax>(
										VariableDeclarator(
											Identifier("count")
										)
										.WithInitializer(
											EqualsValueClause(
												LiteralExpression(
													SyntaxKind.NumericLiteralExpression,
													Literal(0)
												)
											)
										)
									)
								)
							)
							.WithCondition(
								BinaryExpression(
									SyntaxKind.LessThanExpression,
									IdentifierName("count"),
									timesExpression
								)
							)
							.WithIncrementors(
								SingletonSeparatedList<ExpressionSyntax>(
									PostfixUnaryExpression(
										SyntaxKind.PostIncrementExpression,
										IdentifierName("count")
									)
								)
							);

			return Statement(forStatement, base.Generate(context), context);
		}
	}

}