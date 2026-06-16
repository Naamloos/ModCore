using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Components;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Language;
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

        public string Welcome { get; init; }
        public string MainDeveloper { get; init; }
        public string Contribute { get; init; }
        public string Donate { get; init; }
        public string PreviousContributors { get; init; }
        public string Version { get; init; } = "-# ModCore v3-Alpha";

        public AboutViewModel(User modCoreSelf, string language)
        {
            AvatarMedia = new UnfurledMediaItem()
            {
                Url = $"https://cdn.discordapp.com/avatars/{modCoreSelf.Id}/{modCoreSelf.AvatarHash}.png"
            };

            //Welcome = i18n.t("about.welcome", language);
            //MainDeveloper = i18n.t("about.main_developer", language);
            //Contribute = i18n.t("about.contribute", language, new (){
            //    { "repo", "https://github.com/Naamloos/ModCore" }
            //});
            //Donate = i18n.t("about.donate", language, new()
            //{
            //    { "kofi", "https://ko-fi.com/naamloos" }
            //});

            //PreviousContributors = i18n.t("about.previous_contribs", language, new() 
            //{
            //    { "contribs", string.Join(", ", previousContribList.Select(x => $"[{x.Key}]({x.Value})")) }
            //});
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
