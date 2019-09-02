using DSharpPlus.CommandsNext;
using IronBlock;
using IronBlock.Blocks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.CustomCommands
{
    public class BlocklyInterpreter
    {
        public Parser _parser;

        public BlocklyInterpreter()
        {
            _parser = new Parser();
            _parser.AddStandardBlocks();
            _parser.AddBlock<RespondBlock>("RESPOND");
        }

        public async Task InterpretAsync(string xml, CommandContext context)
        {
            var ws = _parser.Parse(xml);

            var dependencies = new DependencyBlockly(context);

            await ws.EvaluateAsync(dependencies);
        }
    }

    public class DependencyBlockly : IDependency
    {
        public DependencyBlockly(CommandContext cc)
        {
            this.CommandContext = cc;
        }
        public CommandContext CommandContext;
    }
}
