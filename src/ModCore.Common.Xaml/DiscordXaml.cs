using ModCore.Common.Discord.Entities.Components;
using Portable.Xaml;
using System.Reflection;

namespace ModCore.Common.Xaml
{
    public class DiscordXaml
    {
        private readonly Assembly parentAssembly;

        public DiscordXaml(Assembly assembly)
        {
            parentAssembly = assembly;
        }

        public async ValueTask<IReadOnlyList<Component>> CompileXamlAsync(string resourceName, object? bindingContext)
        {
            await Task.Yield();

            using var stream = parentAssembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new Exception($"XAML resource '{resourceName}' not found.");

            // HACK - Set the BindingContext for the XAML parser, locally to this async context.
            Binding.BindingContext.Value = bindingContext;

            var xamlObject = XamlServices.Load(stream);

            return ((DiscordView)xamlObject).GetComponents();
        }
    }
}
