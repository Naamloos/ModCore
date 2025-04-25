using ModCore.Common.Discord.Entities.Guilds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Xaml
{
    public class SampleViewModel
    {
        public string ThirdButtonText { get; set; } = "Third Button";
        public Emoji ButtonEmoji { get; set; }

        public SampleViewModel(string thirdButtonText, Emoji buttonEmoji)
        {
            ThirdButtonText = thirdButtonText;
            ButtonEmoji = buttonEmoji;
        }
    }
}
