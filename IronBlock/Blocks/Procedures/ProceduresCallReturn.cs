using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Text
{
    public class ProceduresCallReturn : ProceduresCallNoReturn
    {
        public override async Task<object> EvaluateAsync(Context context)
        {
            // todo: add guard for missing name

            var name = this.Mutations.GetValue("name");
          
            if (!context.Functions.ContainsKey(name)) throw new MissingMethodException($"Method '{name}' not defined");

            var statement = (IFragment)context.Functions[name];

            var funcContext = new Context(context.Dependency) { Parent = context };
            funcContext.Functions = context.Functions;
            
            var counter = 0;
            foreach (var mutation in this.Mutations.Where(x => x.Domain == "arg" && x.Name == "name"))
            {   
                var value = await this.Values.EvaluateAsync($"ARG{counter}", context);
                funcContext.Variables.Add(mutation.Value, value);
                counter++;
            }

            return await statement.EvaluateAsync(funcContext);
        }
	}
}