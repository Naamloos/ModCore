using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ModCore.Common.Discord.Entities.Components;

namespace ModCore.Common.Views.Nodes
{
    [HtmlTargetElement("section", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class SectionNode : NodeBaseWithChildren<Section> { }
}
