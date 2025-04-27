using ModCore.Common.Discord.Entities;
using Portable.Xaml.Markup;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ModCore.Common.Xaml
{
    [MarkupExtensionReturnType(typeof(object))]
    [XamlSetMarkupExtension("Binding")]
    public class Binding : MarkupExtension
    {
        [ConstructorArgument("path")]
        public string Path { get; set; }

        public Binding() { }

        public Binding(string path)
        {
            Path = path;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // Can't do anything without a service provider
            if (serviceProvider == null)
                return null!;

            // Retrieve the IProvideValueTarget service    
            var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (provideValueTarget == null)
                return null!;

            // Ensure the target object and property are valid    
            var targetObject = provideValueTarget.TargetObject;
            var targetProperty = provideValueTarget.TargetProperty as PropertyInfo;
            if (targetObject == null || targetProperty == null)
                return null!;

            // Use BindingContext.Value for runtime binding resolution    
            var bindingContext = BindingContext.Value;
            if (bindingContext == null)
                return null!;

            // Resolve the binding value dynamically    
            var propertyInfo = bindingContext.GetType().GetProperty(Path);
            if (propertyInfo == null)
                return null!;

            // Get the value from the binding context.
            object value = propertyInfo.GetValue(bindingContext);

            // If we're working with an Optional<T>, we need some special hax.
            if (targetProperty.PropertyType.IsGenericType && targetProperty.PropertyType.GetGenericTypeDefinition() == typeof(Optional<>))
            {
                // Get generic argument type
                var genericArgument = targetProperty.PropertyType.GetGenericArguments()[0];
                // Create a new Optional<T> instance with the resolved value  
                var optionalType = typeof(Optional<>).MakeGenericType(genericArgument);
                // It shouldn't matter whether this is a Nullable type or not, but this resolves out issues with Optionals.
                return Activator.CreateInstance(optionalType, value)!;
            }

            return value ?? null!;
        }

        public static AsyncLocal<object?> BindingContext = new AsyncLocal<object?>();
    }
}
