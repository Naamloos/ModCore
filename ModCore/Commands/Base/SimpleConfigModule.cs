using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using ModCore.Entities;
using ModCore.Logic;

namespace ModCore.Commands.Base
{
    public abstract class SimpleConfigModule : BaseCommandModule
    {
        protected virtual string CurrentModuleName => GetType().GetCustomAttribute<GroupAttribute>().Name;

        protected virtual string EnabledState => "Enabled";
        protected virtual string DisabledState => "Disabled";

        [GroupCommand, Description("Sets whether this module is enabled or not.")]
        public async Task ExecuteGroupAsync(CommandContext ctx, [Description(
            "Leave empty to toggle, set to one of `on`, `enable`, `enabled`, `1`, `true`, `yes` or `y` to enable, or " +
            "set to one of `off`, `disable`, `disabled`, `0`, `false`, `no` or `n` to disable. "
        )] bool? enableOrDisable = null)
        {
            // we can't access ref inside an async method, so make a copy
            var resultingVariable = false;

            await ctx.WithGuildSettings(cfg =>
            {
                ref var configVariable = ref GetSetting(cfg);

                resultingVariable = configVariable = enableOrDisable ?? !configVariable;
            });

            if (resultingVariable)
                await AfterEnable(ctx);
            else
                await AfterDisable(ctx);
            
            // if toggling, tell the user what the new value is
            if (!enableOrDisable.HasValue)
                await ctx.ElevatedRespondAsync(
                    $"**{(resultingVariable ? EnabledState : DisabledState)}** the {CurrentModuleName} module.");
            
            await ctx.Message.CreateReactionAsync(Config.CheckMark);
        }

        /// <summary>
        /// Implementations of this method should return a reference to the field to be set.
        /// </summary>
        /// <param name="cfg">The <see cref="GuildSettings"/> object the setting is tied to</param>
        /// <returns>A reference to a field to be set by <see cref="ExecuteGroupAsync"/></returns>
        protected abstract ref bool GetSetting(GuildSettings cfg);

        protected virtual Task AfterEnable(CommandContext ctx) => Task.CompletedTask;

        protected virtual Task AfterDisable(CommandContext ctx) => Task.CompletedTask;
    }
}