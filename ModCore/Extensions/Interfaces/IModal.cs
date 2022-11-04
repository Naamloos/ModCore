using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace ModCore.Extensions.Interfaces
{
    public interface IModal
    {
        Task HandleAsync(DiscordInteraction interaction);
    }
}
