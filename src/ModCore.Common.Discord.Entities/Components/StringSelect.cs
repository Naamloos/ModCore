using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Guilds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities.Components
{
    public class StringSelect : InteractiveComponent
    {
        public ComponentType Type { get; set; } = ComponentType.StringSelect;

        public List<SelectOption> Options { get; set; } = new List<SelectOption>();

        public string? Placeholder { get; set; } = null;

        public int? MinValues { get; set; } = null;

        public int? MaxValues { get; set; } = null;

        public bool? Disabled { get; set; } = false;
    }

    public class SelectOption
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Description { get; set; } = null;
        public Emoji? Emoji { get; set; } = null;
        public bool? Default { get; set; } = false;
    }
}
