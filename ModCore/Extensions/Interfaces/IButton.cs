using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace ModCore.Extensions.Interfaces
{
    public interface IButton
    {
        Task HandleAsync(DiscordInteraction interaction, DiscordMessage message);
    }
}
