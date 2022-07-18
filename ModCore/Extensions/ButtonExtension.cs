using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Extensions.Buttons.Attributes;
using ModCore.Extensions.Buttons.Interfaces;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ModCore.Extensions
{
    public class ButtonExtension : BaseExtension
    {
        private readonly ConcurrentBag<ButtonHandler> handlers;

        private readonly IServiceProvider services;

        public ButtonExtension(IServiceProvider services)
        {
            this.services = services;
            this.handlers = new ConcurrentBag<ButtonHandler>();
        }

        protected override void Setup(DiscordClient client)
            => client.ComponentInteractionCreated += (sender, e) =>
            {
                _ = Task.Run(async () => await handleButtonAsync(sender, e));
                return Task.CompletedTask;
            };

        public string GenerateCommand<T>(IDictionary<string, string> values) where T : IButton
        {
            var handler = handlers.FirstOrDefault(x => x.HandlerType == typeof(T));
            if (handler == null)
            {
                // Cache for next call :^)
                handler = RegisterHandler(typeof(T));
            }

            return handler.Create(values);
        }

        private ButtonHandler RegisterHandler(Type type)
        {
            var buttonAttributes = type.GetCustomAttributes(typeof(ButtonAttribute), false);
            if (buttonAttributes.Length != 1)
            {
                throw new Exception($"{type} has no {typeof(ButtonAttribute)}!");
            }

            var buttonAttribute = buttonAttributes[0] as ButtonAttribute;

            if (type.GetConstructors().Length != 1)
                throw new Exception($"{type} has more than 1 constructor!");

            if (handlers.Any(x => x.HandlerType == type))
                throw new Exception($"Modal with type {type} already registered!");

            var handler = new ButtonHandler(buttonAttribute, type);
            handlers.Add(handler);

            return handler;
        }

        private async Task handleButtonAsync(DiscordClient sender, ComponentInteractionCreateEventArgs e)
        {
            if(e.Interaction.Data.ComponentType == ComponentType.Button)
            {
                var deciphered = ExtensionStatics.DecipherIdString(e.Interaction.Data.CustomId);

                var handler = handlers.FirstOrDefault(x => x.Data.Id == deciphered.Id);
                if (handler != null)
                    await handler.ExecuteAsync(e, deciphered.Values, services);
            }
        }
    }

    class ButtonHandler
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

            await button.HandleAsync(e.Interaction);
        }
    }
}
