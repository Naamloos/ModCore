using IronBlock.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Lists
{
	public class ListsIndexOf : IBlock
	{
        public override async Task<object> EvaluateAsync(Context context)
		{
			var direction = this.Fields.Get("END");
			var value = await this.Values.EvaluateAsync("VALUE", context) as IEnumerable<object>;
			var find = await this.Values.EvaluateAsync("FIND", context);

			switch (direction)
			{
				case "FIRST": 
					return value.ToList().IndexOf(find) + 1;
				
				case "LAST": 
					return value.ToList().LastIndexOf(find) + 1;

				default:
					throw new NotSupportedException("$Unknown end: {direction}");
			}
		}

		public override SyntaxNode Generate(Context context)
		{
			throw new NotImplementedException();
		}
	}
}