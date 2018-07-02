using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ModCore.Entities;

namespace ModCore.Commands.Base
{
    public abstract class ExemptModule<T> where T : SnowflakeObject
    {
        [Command("exempt"), Aliases("ignore", "x", "i"), Description("Exempts something from this module.")]
        public async Task ExemptAsync(CommandContext ctx,
            [RemainingText, Description("What to exempt from this module")] T obj)
        {
            await ctx.WithGuildSettings(cfg => cfg.Linkfilter.ExemptUserIds.Add(obj.Id));
            await AfterExemptAsync(ctx, obj);
            await ctx.Message.CreateReactionAsync(Config.CheckMark);
        }

        [Command("unexempt"), Aliases("unignore", "ux", "u"), Description("Unexempts something from this module.")]
        public async Task UnexemptAsync(CommandContext ctx,
            [RemainingText, Description("What to unexempt from this module")] T obj)
        {
            await ctx.WithGuildSettings(cfg => GetExemptionList(cfg).Remove(obj.Id));
            await AfterUnexemptAsync(ctx, obj);
            await ctx.Message.CreateReactionAsync(Config.CheckMark);
        }

        protected abstract ISet<ulong> GetExemptionList(GuildSettings cfg);

        protected virtual Task AfterExemptAsync(CommandContext ctx, T obj) => Task.CompletedTask;
        protected virtual Task AfterUnexemptAsync(CommandContext ctx, T obj) => Task.CompletedTask;
    }
}