using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Components;
using ModCore.Common.Discord.Entities.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Services.Shard.Modules.About.ViewModels
{
    public class AboutViewModel
    {
        public int AccentColor { get; init; } = 0x089FDF;
        public UnfurledMediaItem AvatarMedia { get; init; }
        public string PreviousContributors { get; init; } = "";
        public string Version { get; init; } = "-# ModCore v3-Alpha";

        public AboutViewModel(User modCoreSelf)
        {
            AvatarMedia = new UnfurledMediaItem()
            {
                Url = $"https://cdn.discordapp.com/avatars/{modCoreSelf.Id}/{modCoreSelf.AvatarHash}.png"
            };
            PreviousContributors = "**Special thanks to all of these wonderful previous contributors:**\n"
                + string.Join(", ", previousContribList.Select(x => $"[{x.Key}]({x.Value})"));
        }

        private static Dictionary<string, string> previousContribList = new()
        {
            { "uwx", "https://github.com/uwx" },
            { "jcryer", "https://github.com/jcryer" },
            { "Emzi0767", "https://github.com/Emzi0767" },
            { "YourAverageBlackGuy", "https://github.com/YourAverageBlackGuy" },
            { "DrCreo", "https://github.com/DrCreo" },
            { "aexolate", "https://github.com/aexolate" },
            { "Drake103", "https://github.com/Drake103" },
            { "Izumemori", "https://github.com/Izumemori" },
            { "OoLunar", "https://github.com/OoLunar" },
            { "InFTord", "https://github.com/InFTord" }
        };
    }
}
