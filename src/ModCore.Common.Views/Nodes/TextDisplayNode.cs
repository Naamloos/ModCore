using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ModCore.Common.Discord.Entities.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Views.Nodes
{
    [HtmlTargetElement("text", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class TextDisplayNode : TagHelper
    {
        [ViewContext] public ViewContext ViewContext { get; set; } = default!;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var dctx = (RazorBuildContext)ViewContext.HttpContext.Items[nameof(RazorBuildContext)]!;
            var rawContent = (await output.GetChildContentAsync()).GetContent();
            // Decode HTML entities except for [] and ()
            var content = System.Net.WebUtility.HtmlDecode(rawContent)
                .Replace("&#91;", "[")
                .Replace("&#93;", "]")
                .Replace("&#40;", "(")
                .Replace("&#41;", ")");
            var textDisplay = new TextDisplay
            {
                Content = content
            };
            dctx.ComponentStack.Add(textDisplay);
            dctx.Previous = dctx.ComponentStack.Last();
            output.SuppressOutput();
        }
    }
}
