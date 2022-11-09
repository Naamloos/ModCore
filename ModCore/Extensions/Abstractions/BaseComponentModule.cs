using DSharpPlus;

namespace ModCore.Extensions.Abstractions
{
    public abstract class BaseComponentModule
    {
        public DiscordClient Client { protected get; set; }
    }
}
