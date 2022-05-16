using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;
using ModCore.Logic.Extensions;
using ModCore.Logic.Utils.Captcha;

namespace ModCore.Commands
{
    public partial class ConfigNew
    {
        [Command("setup"), Description("Sets up this guild's config.")]
        [RequireBotPermissions(Permissions.ManageWebhooks | Permissions.ManageRoles | Permissions.ManageGuild)]
        public async Task SetupAsync(CommandContext context)
        {
            // TODO setup utility.
            await context.RespondAsync("⌛ WIP: This feature has not been implemented yet.");
        }
    }
}
