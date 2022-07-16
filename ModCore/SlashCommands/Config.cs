using DSharpPlus;
using DSharpPlus.SlashCommands;
using System.Threading.Tasks;

namespace ModCore.SlashCommands
{
    [SlashCommandGroup("config", "Server configuration commands.")]
    [SlashCommandPermissions(Permissions.ManageGuild)]
    public class Config
    {
        // Linkfilter, Rolestates, Logging, Autorole, Selfrole, Starboard, Rolemenu, Welcomer, Nicknamerequests, Levels
        [SlashCommandGroup("filters", "Chat filter configuration.")]
        public class Filters
        {

        }

        [SlashCommandGroup("rolestate", "Role state configration.")]
        public class Rolestate
        {

        }

        [SlashCommandGroup("logging", "Logging configuration.")]
        public class Logging
        {

        }

        [SlashCommandGroup("autorole", "Autorole configuration.")]
        public class Autorole
        {

        }

        [SlashCommandGroup("selfrole", "Selfrole configuration.")]
        public class Selfrole
        {

        }

        [SlashCommandGroup("starboard", "Starboard configuration.")]
        public class Starboard
        {

        }

        [SlashCommandGroup("welcome", "Welcome message configuration.")]
        public class Welcome
        {

        }

        [SlashCommandGroup("nicknamerequest", "Nickname request configuration.")]
        public class NicknameRequest
        {

        }

        [SlashCommandGroup("levels", "Level configuration.")]
        public class Levels
        {

        }
    }
}
