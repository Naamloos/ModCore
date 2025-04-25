using Portable.Xaml.Markup;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ModCore.Common.Xaml
{
    [MarkupExtensionReturnType(typeof(object))]
    [TypeForwardedFrom("PresentationFramework.dll")]
    public class Binding : MarkupExtension
    {
        public string Path { get; set; }

        public Binding(string path)
        {
            Path = path;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            // Retrieve the IProvideValueTarget service  
            var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (provideValueTarget == null)
                throw new InvalidOperationException("IProvideValueTarget service is not available.");

            // Ensure the target object and property are valid  
            var targetObject = provideValueTarget.TargetObject;
            var targetProperty = provideValueTarget.TargetProperty;
            if (targetObject == null || targetProperty == null)
                throw new InvalidOperationException("Target object or property is null.");

            // Use BindingContext.Value for runtime binding resolution  
            var bindingContext = BindingContext.Value;
            if (bindingContext == null)
                return $"Unable to resolve binding {Path}";

            // Resolve the binding value dynamically  
            var propertyInfo = bindingContext.GetType().GetProperty(Path);
            if (propertyInfo == null)
                throw new InvalidOperationException($"Property '{Path}' not found on binding context.");

            return propertyInfo.GetValue(bindingContext);
        }

        public static AsyncLocal<object> BindingContext = new AsyncLocal<object>();
    }
}
