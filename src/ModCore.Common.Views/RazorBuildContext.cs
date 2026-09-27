using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModCore.Common.Discord.Entities.Components;

namespace ModCore.Common.Views
{
    public sealed class RazorBuildContext
    {
        public List<Component> ComponentStack { get; } = new();
        public Component? Previous;
    }
}
