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

    public class SectionAccessory : TagHelper
    {
        [ViewContext] public ViewContext ViewContext { get; set; } = default!;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var dctx = (RazorBuildContext)ViewContext.HttpContext.Items[nameof(RazorBuildContext)]!;
            if(dctx.Previous?.GetType() != typeof(ModCore.Common.Discord.Entities.Components.Section))
            {
                throw new InvalidOperationException("SectionAccessory must be used directly after a Section component.");
            }
            var section = dctx.Previous as Section;
            int stackStart = dctx.ComponentStack.Count;
            await output.GetChildContentAsync();
            int stackEnd = dctx.ComponentStack.Count;
            if(stackEnd - stackStart != 1)
            {
                throw new InvalidOperationException("SectionAccessory must have exactly one child component.");
            }
            var accessory = dctx.ComponentStack.Last();
            section.Accessory = accessory;
            dctx.ComponentStack.RemoveAt(stackEnd - 1);
            dctx.Previous = section;
            output.SuppressOutput();
        }
    }
}
