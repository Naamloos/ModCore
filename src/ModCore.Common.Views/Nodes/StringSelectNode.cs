using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ModCore.Common.Discord.Entities.Components;
using System.Threading.Tasks;

namespace ModCore.Common.Views.Nodes
{
    [HtmlTargetElement("stringselect", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class StringSelectNode : TagHelper
    {
        [ViewContext] public ViewContext ViewContext { get; set; } = default!;

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var dctx = (RazorBuildContext)ViewContext.HttpContext.Items[nameof(RazorBuildContext)]!;
            dctx.ComponentStack.Add(new StringSelect());
            dctx.Previous = dctx.ComponentStack[^1];
            output.SuppressOutput();
            return Task.CompletedTask;
        }
    }
}
