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
    public abstract class NodeBase<T> : TagHelper where T : Component
    {
        [ViewContext] public ViewContext ViewContext { get; set; } = default!;

        public virtual Task<T> ProcessContentAndProps(T component, TagHelperContext context, TagHelperOutput output) { return Task.FromResult(component); }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var dctx = (RazorBuildContext)ViewContext.HttpContext.Items[nameof(RazorBuildContext)]!;
            int stackStart = dctx.ComponentStack.Count;
            var component = Activator.CreateInstance<T>();
            component = await ProcessContentAndProps(component, context, output);
            dctx.ComponentStack.Add(component);
            dctx.Previous = component;
            output.SuppressOutput();
        }
    }
}
