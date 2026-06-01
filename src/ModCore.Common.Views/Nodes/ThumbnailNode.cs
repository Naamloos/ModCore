using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ModCore.Common.Discord.Entities.Components;
using System.Threading.Tasks;

namespace ModCore.Common.Views.Nodes
{
    [HtmlTargetElement("thumbnail", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class ThumbnailNode : NodeBase<Thumbnail>
    {
        public required string Url { get; set; }

        public override Task<Thumbnail> ProcessContentAndProps(Thumbnail component, TagHelperContext context, TagHelperOutput output)
        {
            component.Media = new UnfurledMediaItem()
            {
                Url = this.Url,
            };

            return Task.FromResult(component);
        }
    }
}
