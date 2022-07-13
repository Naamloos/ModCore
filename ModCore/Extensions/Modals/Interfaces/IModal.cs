using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace ModCore.Extensions.Modals.Interfaces
{
    public interface IModal
    {
        Task HandleAsync(DiscordInteraction interaction);
    }
}
