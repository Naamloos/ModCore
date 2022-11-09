using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using ModCore.Entities;
using ModCore.Utils.Extensions;

namespace ModCore.OldCommands
{
    public class Eval : BaseCommandModule
	{
		public SharedData shared;

		public Eval(SharedData shared)
		{
			this.shared = shared;
		}

		[Command("eval"), Aliases("evalcs", "cseval", "roslyn"), Description("Evaluates C# code."), Hidden, RequireOwner]
		public async Task EvalCS(CommandContext context, [RemainingText] string code)
		{
			var message = context.Message;

			var code_start = code.IndexOf("```") + 3;
			code_start = code.IndexOf('\n', code_start) + 1;
			var code_end = code.LastIndexOf("```");

			if (code_start == -1 || code_end == -1)
				throw new ArgumentException("⚠️ You need to wrap the code into a code block.");

			var cs = code.Substring(code_start, code_end - code_start);

			message = await context.ElevatedRespondAsync(embed: new DiscordEmbedBuilder()
				.WithColor(new DiscordColor("#FF007F"))
				.WithDescription("💭 Evaluating...")
				.Build()).ConfigureAwait(false);

			try
			{
				var globals = new TestVariables(context.Message, context.Client, context, shared.ModCore, shared);

				var scriptoptions = ScriptOptions.Default;
				scriptoptions = scriptoptions.WithImports("System", "System.Collections.Generic", "System.Linq", "System.Text", "System.Threading.Tasks", "DSharpPlus", "DSharpPlus.CommandsNext", "DSharpPlus.Interactivity", "ModCore");
				scriptoptions = scriptoptions.WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(xa => !xa.IsDynamic && !string.IsNullOrWhiteSpace(xa.Location)));

				var script = CSharpScript.Create(cs, scriptoptions, typeof(TestVariables));
				script.Compile();
				var result = await script.RunAsync(globals).ConfigureAwait(false);

				if (result != null && result.ReturnValue != null && !string.IsNullOrWhiteSpace(result.ReturnValue.ToString()))
					await message.ModifyAsync(embed: new DiscordEmbedBuilder { Title = "✅ Evaluation Result", Description = result.ReturnValue.ToString(), Color = new DiscordColor("#089FDF") }.Build()).ConfigureAwait(false);
				else
					await message.ModifyAsync(embed: new DiscordEmbedBuilder { Title = "✅ Evaluation Successful", Description = "No result was returned.", Color = new DiscordColor("#089FDF") }.Build()).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				await message.ModifyAsync(embed: new DiscordEmbedBuilder { Title = "⚠️ Evaluation Failure", Description = string.Concat("**", ex.GetType().ToString(), "**: ", ex.Message), Color = new DiscordColor("#FF0000") }.Build()).ConfigureAwait(false);
			}
		}
	}

	public class TestVariables
	{
		public DiscordMessage Message { get; set; }
		public DiscordChannel Channel { get; set; }
		public DiscordGuild Guild { get; set; }
		public DiscordUser User { get; set; }
		public DiscordMember Member { get; set; }
		public CommandContext Context { get; set; }

		public TestVariables(DiscordMessage msg, DiscordClient client, CommandContext ctx, ModCore core, SharedData share)
		{
			this.Client = client;

			this.Message = msg;
			this.Channel = msg.Channel;
			this.Guild = this.Channel.Guild;
			this.User = this.Message.Author;
			if (this.Guild != null)
				this.Member = this.Guild.GetMemberAsync(this.User.Id).ConfigureAwait(false).GetAwaiter().GetResult();
			this.Context = ctx;
			this.ModCore = core;
			this.SharedData = share;
		}

		public DiscordClient Client;
		public ModCore ModCore;
		public SharedData SharedData;
	}
}
