using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Variables
{
    public class VariablesGet : IBlock
    {
        public override async Task<object> EvaluateAsync(Context context)
        {
            var variableName = this.Fields.Get("VAR");

            if (!context.Variables.ContainsKey(variableName)) return null;
			
            return context.Variables[variableName];
        }

		public override SyntaxNode Generate(Context context)
		{
			var variableName = this.Fields.Get("VAR").CreateValidName();

			return IdentifierName(variableName);
		}
	}

}