using ModCore.Common.Discord.Entities.Components;
using Portable.Xaml.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Xaml
{
    [ContentProperty(nameof(Components))]
    public class DiscordView : IComponentConnector
    {
        internal object ViewModel { get; set; } = null;

        internal List<Component> Components { get; set; } = new List<Component>();

        public void Connect(int connectionId, object target)
        {
        }

        public void InitializeComponent()
        {
        }

        internal IReadOnlyList<Component> GetComponents()
        {
            return Components.AsReadOnly();
        }
    }
}
