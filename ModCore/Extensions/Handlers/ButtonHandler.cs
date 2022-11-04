using DSharpPlus.EventArgs;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System;
using System.Linq;
using ModCore.Extensions.Attributes;
using ModCore.Extensions.Interfaces;

namespace ModCore.Extensions.Handlers
{
    public class ButtonHandler
    {
        public ButtonAttribute Data { get; private set; }
        public Type HandlerType { get; private set; }
        private readonly ConcurrentDictionary<ButtonFieldAttribute, PropertyInfo> fields;

        public ButtonHandler(ButtonAttribute data, Type handlerType)
        {
            Data = data;
            HandlerType = handlerType;

            this.fields = new ConcurrentDictionary<ButtonFieldAttribute, PropertyInfo>();
            var properties = handlerType.GetProperties().Where(x => x.GetSetMethod() != null);
            var fields = properties.Where(x => x.GetCustomAttribute<ButtonFieldAttribute>() != null);

            foreach (var property in fields)
                this.fields.TryAdd(property.GetCustomAttribute<ButtonFieldAttribute>(), property);
        }

        public string Create(IDictionary<string, string> values)
        {
            return ExtensionStatics.GenerateIdString(Data.Id, values);
        }

        public async Task ExecuteAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> values, IServiceProvider services)
        {
            // Construct new modal with dependencies injected into constructor
            IEnumerable<object> constructorValues = HandlerType.GetConstructors()[0].GetParameters().Select(x => services.GetService(x.ParameterType));
            var button = (IButton)Activator.CreateInstance(HandlerType, constructorValues.ToArray());

            // Inject visible modal fields
            foreach (var field in values)
            {
                fields.First(x => x.Key.Name == field.Key).Value.SetValue(button, field.Value);
            }

            await button.HandleAsync(e.Interaction, e.Message);
        }
    }
}
