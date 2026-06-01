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
    [HtmlTargetElement("actionrow", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class ActionRowNode : NodeBaseWithChildren<ActionRow>
    {
    }
}
