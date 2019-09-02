using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace IronBlock.Blocks.Controls
{
	public class ControlsForEach : IBlock
    {
        public override async Task<object> EvaluateAsync(Context context)
        {
            var variableName = this.Fields.Get("VAR");
            var list = await this.Values.EvaluateAsync("LIST",context) as IEnumerable<object>;

            var statement = this.Statements.Where(x => x.Name == "DO").FirstOrDefault();

            if (null == statement) return await base.EvaluateAsync(context);

            foreach (var item in list)
            {
                if (context.Variables.ContainsKey(variableName))
                {
                    context.Variables[variableName] = item;
                }
                else
                {
                    context.Variables.Add(variableName, item);
                }
                await statement.EvaluateAsync(context);
            }

            return await base.EvaluateAsync(context);
        }

		public override SyntaxNode Generate(Context context)
		{
			var variableName = this.Fields.Get("VAR").CreateValidName();
			var listExpression = this.Values.Generate("LIST", context) as ExpressionSyntax;
			if (listExpression == null) throw new ApplicationException($"Unknown expression for list.");

			var statement = this.Statements.Where(x => x.Name == "DO").FirstOrDefault();

			if (null == statement) return base.Generate(context);

			var forEachContext = new Context(context.Dependency) { Parent = context };
			if (statement?.Block != null)
			{
				var statementSyntax = statement.Block.GenerateStatement(forEachContext);
				if (statementSyntax != null)
				{
					forEachContext.Statements.Add(statementSyntax);
				}
			}

			var forEachStatement =
					ForEachStatement(
							IdentifierName("var"),
							Identifier(variableName),
							listExpression,
							Block(
								forEachContext.Statements
							)
						);

			return Statement(forEachStatement, base.Generate(context), context);
		}
	}

}