using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Gateway
{
    // These are taken from DSharpPlus because of laziness
    [Flags]
    public enum Intents
    {
        Guilds = 1 << 0,

        GuildMembers = 1 << 1,

        GuildBans = 1 << 2,

        GuildEmojis = 1 << 3,

        GuildIntegrations = 1 << 4,

        GuildWebhooks = 1 << 5,

        GuildInvites = 1 << 6,

        GuildVoiceStates = 1 << 7,

        GuildPresences = 1 << 8,

        GuildMessages = 1 << 9,

        GuildMessageReactions = 1 << 10,

        GuildMessageTyping = 1 << 11,

        DirectMessages = 1 << 12,

        DirectMessageReactions = 1 << 13,

        DirectMessageTyping = 1 << 14,

        MessageContents = 1 << 15,

        ScheduledGuildEvents = 1 << 16,

        AutoModerationEvents = 1 << 20,

        AutoModerationExecution = 1 << 21,

        AllUnprivileged = Guilds | GuildBans | GuildEmojis | GuildIntegrations | GuildWebhooks | GuildInvites | GuildVoiceStates | GuildMessages |
            GuildMessageReactions | GuildMessageTyping | DirectMessages | DirectMessageReactions | DirectMessageTyping | ScheduledGuildEvents |
            AutoModerationEvents | AutoModerationExecution,

        All = AllUnprivileged | GuildMembers | GuildPresences | MessageContents
    }
}
