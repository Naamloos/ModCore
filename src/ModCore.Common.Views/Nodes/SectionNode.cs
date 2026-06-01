using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ModCore.Common.Discord.Entities.Components;
using System.Threading.Tasks;

namespace ModCore.Common.Views.Nodes
{
    [HtmlTargetElement("section", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class SectionNode : NodeBaseWithChildren<Section>
    {
    }
}
