using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ModCore.Common.Discord.Entities.Components;

namespace ModCore.Common.Views.Nodes
{
    [HtmlTargetElement("container", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class ContainerNode : NodeBaseWithChildren<Container>
    {
        public string? Color { get; set; }

        public override async Task<Container> ProcessContentAndProps(
            Container component,
            TagHelperContext context,
            TagHelperOutput output
        )
        {
            if (Color != null)
            {
                if (Color.StartsWith("#"))
                {
                    Color = Color[1..];
                }
                if (
                    int.TryParse(
                        Color,
                        System.Globalization.NumberStyles.HexNumber,
                        null,
                        out int colorInt
                    )
                )
                {
                    component.AccentColor = colorInt;
                }
            }

            return component;
        }
    }
}
