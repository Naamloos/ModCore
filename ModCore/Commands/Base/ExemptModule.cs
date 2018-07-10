using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ModCore.Entities;

namespace ModCore.Commands.Base
{
    public abstract class BaseExemptModule<T> : BaseCommandModule where T : SnowflakeObject
    {
        protected async Task BaseExemptAsync(CommandContext ctx, T obj)
        {
            await ctx.WithGuildSettings(cfg => GetExemptionList(cfg).Add(obj.Id));
            await AfterExemptAsync(ctx, obj);
            await ctx.Message.CreateReactionAsync(Config.CheckMark);
        }

        public async Task BaseUnexemptAsync(CommandContext ctx, T obj)
        {
            await ctx.WithGuildSettings(cfg => GetExemptionList(cfg).Remove(obj.Id));
            await AfterUnexemptAsync(ctx, obj);
            await ctx.Message.CreateReactionAsync(Config.CheckMark);
        }
        
        protected abstract ISet<ulong> GetExemptionList(GuildSettings cfg);

        protected virtual Task AfterExemptAsync(CommandContext ctx, T obj) => Task.CompletedTask;
        protected virtual Task AfterUnexemptAsync(CommandContext ctx, T obj) => Task.CompletedTask;
    }

    public abstract class ExemptRoleModule : BaseExemptModule<DiscordRole>
    {
        [Command("exempt"), Aliases("ignore", "x", "i"), Description("Exempts a role from this module.")]
        public Task ExemptAsync(CommandContext ctx,
            [RemainingText, Description("What role to exempt from this module")] DiscordRole obj)
            => BaseExemptAsync(ctx, obj);

        [Command("unexempt"), Aliases("unignore", "ux", "u"), Description("Unexempts a role from this module.")]
        public Task UnexemptAsync(CommandContext ctx,
            [RemainingText, Description("What role to unexempt from this module")] DiscordRole obj)
            => BaseExemptAsync(ctx, obj);
    }

    public abstract class ExemptMemberModule : BaseExemptModule<DiscordMember>
    {
        [Command("exempt"), Aliases("ignore", "x", "i"), Description("Exempts a member from this module.")]
        public Task ExemptAsync(CommandContext ctx,
            [RemainingText, Description("Who to exempt from this module")] DiscordMember obj)
            => BaseExemptAsync(ctx, obj);

        [Command("unexempt"), Aliases("unignore", "ux", "u"), Description("Unexempts a member from this module.")]
        public Task UnexemptAsync(CommandContext ctx,
            [RemainingText, Description("Who to unexempt from this module")] DiscordMember obj)
            => BaseExemptAsync(ctx, obj);
    }
    public abstract class ExemptChannelModule : BaseExemptModule<DiscordChannel>
    {
        [Command("exempt"), Aliases("ignore", "x", "i"), Description("Exempts a channel from this module.")]
        public Task ExemptAsync(CommandContext ctx,
            [RemainingText, Description("What channel to exempt from this module")] DiscordChannel obj)
            => BaseExemptAsync(ctx, obj);

        [Command("unexempt"), Aliases("unignore", "ux", "u"), Description("Unexempts a channel from this module.")]
        public Task UnexemptAsync(CommandContext ctx,
            [RemainingText, Description("What channel to unexempt from this module")] DiscordChannel obj)
            => BaseExemptAsync(ctx, obj);
    }
}