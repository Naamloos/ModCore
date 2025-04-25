using ModCore.Common.Discord.Entities.Components;
using Portable.Xaml;
using System.Reflection;

namespace ModCore.Common.Xaml
{
    public class DiscordXaml
    {
        public async ValueTask<IReadOnlyList<Component>> CompileXamlAsync(string resourceName, object bindingContext, Assembly? parentAssembly = null)
        {
            await Task.Yield();

            var assembly = parentAssembly ?? this.GetType().Assembly;
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new Exception($"XAML resource '{resourceName}' not found.");

            // HACK - Set the BindingContext for the XAML parser, locally to this async context.
            Binding.BindingContext.Value = bindingContext;

            var xamlObject = XamlServices.Load(stream);
            if (xamlObject is DiscordView discordView)
            {
                discordView.ViewModel = bindingContext; // Set the ViewModel
            }

            return ((DiscordView)xamlObject).GetComponents();
        }
    }
}
