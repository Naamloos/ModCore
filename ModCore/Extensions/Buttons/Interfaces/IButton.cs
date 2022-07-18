using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace ModCore.Extensions.Buttons.Interfaces
{
    public interface IButton
    {
        Task HandleAsync(DiscordInteraction interaction);
    }
}
