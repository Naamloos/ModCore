using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ModCore.Common.Discord.Entities.Components;
using ModCore.Common.Discord.Entities.Guilds;
using System.Threading.Tasks;

namespace ModCore.Common.Views.Nodes
{
    [HtmlTargetElement("button", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class ButtonNode : NodeBase<Button>
    {
        [ViewContext] public ViewContext ViewContext { get; set; } = default!;

        public ButtonStyle Style { get; set; } = ButtonStyle.Primary;
        public string? CustomId { get; set; }
        public Emoji? Emoji { get; set; }
        public string? Url { get; set; }
        public bool Disabled { get; set; } = false;

        public override async Task<Button> ProcessContentAndProps(Button component, TagHelperContext context, TagHelperOutput output)
        {
            component.Label = (await output.GetChildContentAsync()).GetContent();
            component.Style = Style;
            if (!string.IsNullOrWhiteSpace(CustomId))
                component.CustomId = CustomId;
            if (Emoji is not null)
                component.Emoji = Emoji;
            if (!string.IsNullOrWhiteSpace(Url))
                component.Url = Url;

            return component;
        }
    }
}
