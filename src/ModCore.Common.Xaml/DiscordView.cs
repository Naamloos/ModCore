using ModCore.Common.Discord.Entities.Components;
using Portable.Xaml.Markup;

namespace ModCore.Common.Xaml
{
    [ContentProperty(nameof(Components))]
    public class DiscordView : IComponentConnector
    {
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
