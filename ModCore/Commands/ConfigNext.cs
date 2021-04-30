using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Commands
{
    [Group("confignext")]
    [Aliases("cfgn")]
    [Description("Guild configuration options.")]
    [RequireUserPermissions(Permissions.ManageGuild)]
    public class ConfigNext : BaseCommandModule
    {
        [GroupCommand]
        [Description("Runs guild config.")]
        public async Task ExecuteGroupAsync(CommandContext ctx)
        {

        }
    }
}
