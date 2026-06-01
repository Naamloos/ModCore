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
    public abstract class NodeBaseWithChildren<T> : NodeBase<T> where T : ComponentWithChildren
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var dctx = (RazorBuildContext)ViewContext.HttpContext.Items[nameof(RazorBuildContext)]!;
            var component = Activator.CreateInstance<T>();
            dctx.Previous = component;
            int stackStart = dctx.ComponentStack.Count;
            await output.GetChildContentAsync();
            int stackEnd = dctx.ComponentStack.Count;
            var children = dctx.ComponentStack.Skip(stackStart).ToList();
            if (children.Count > 0)
            {
                dctx.ComponentStack.RemoveRange(stackStart, children.Count);
            }
            component.Components = children;
            component = await ProcessContentAndProps(component, context, output);
            dctx.ComponentStack.Add(component);
            dctx.Previous = component;
            output.SuppressOutput();
        }
    }
}
