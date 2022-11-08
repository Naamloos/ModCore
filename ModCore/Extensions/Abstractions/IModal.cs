using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace ModCore.Extensions.Abstractions
{
    public interface IModal
    {
        Task HandleAsync(DiscordInteraction interaction);
    }
}
