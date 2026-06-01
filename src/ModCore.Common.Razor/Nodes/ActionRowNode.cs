using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ModCore.Common.Discord.Entities.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Razor.Nodes
{
    [HtmlTargetElement("ActionRow", ParentTag = "d-message", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class ActionRowNode : TagHelper
    {
        [ViewContext] public ViewContext ViewContext { get; set; } = default!;

        public string Test { get; set; } = string.Empty;

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var dctx = (RazorBuildContext)ViewContext.HttpContext.Items[nameof(RazorBuildContext)]!;
            dctx.ComponentStack.Add(new ActionRow());
            dctx.Previous = dctx.ComponentStack.Last();
            output.SuppressOutput();
            return Task.CompletedTask;
        }
    }
}
