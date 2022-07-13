using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModCore.Extensions.Modals.Attributes;
using ModCore.Extensions.Modals.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ModCore.Extensions;
using DSharpPlus.Entities;

namespace ModCore.Extensions
{
    public class ModalExtension : BaseExtension
    {
        private readonly ConcurrentBag<ModalHandler> handlers;

        private readonly IServiceProvider services;

        public ModalExtension(IServiceProvider services)
        {
            this.services = services;
            this.handlers = new ConcurrentBag<ModalHandler>();
        }

        protected override void Setup(DiscordClient client)
            => client.ModalSubmitted += (sender, e) => Task.Run(async () => await handleModalSubmission(sender, e));

        private ModalHandler RegisterHandler(Type type)
        {
            var modalAttributes = type.GetCustomAttributes(typeof(ModalAttribute), false);
            if (modalAttributes.Length != 1)
            {
                throw new Exception($"{type} has no {typeof(ModalAttribute)}!");
            }

            var modalAttribute = modalAttributes[0] as ModalAttribute;

            if (type.GetConstructors().Length != 1)
                throw new Exception($"{type} has more than 1 constructor!");

            if (handlers.Any(x => x.Type == type))
                throw new Exception($"Modal with type {type} already registered!");

            var handler = new ModalHandler(type, modalAttribute);
            handlers.Add(handler);

            return handler;
        }

        public async Task RespondWithModalAsync<T>(DiscordInteraction interaction, string title,
            IDictionary<string, string> hiddenValues = null) where T : IModal
        {
            var handler = handlers.FirstOrDefault(x => x.Type == typeof(T));
            if (handler == null)
            {
                // Cache for next call :^)
                handler = RegisterHandler(typeof(T));
            }

            await handler.CreateAsync(interaction, title, hiddenValues);
        }

        private async Task handleModalSubmission(DiscordClient sender, ModalSubmitEventArgs e)
        {
            var deciphered = ExtensionStatics.DecipherIdString(e.Interaction.Data.CustomId);

            var handler = handlers.FirstOrDefault(x => x.Info.ModalId == deciphered.Id);
            if (handler != null)
                await handler.ExecuteAsync(e, deciphered.Values, services);
        }
    }

    class ModalHandler
    {
        public Type Type { get; private set; }

        public ModalAttribute Info { get; private set; }

        private readonly ConcurrentDictionary<ModalFieldAttribute, PropertyInfo> fields;

        private readonly ConcurrentDictionary<ModalHiddenFieldAttribute, PropertyInfo> hiddenFields;

        public ModalHandler(Type type, ModalAttribute info)
        {
            this.Type = type;
            this.Info = info;
            this.hiddenFields = new ConcurrentDictionary<ModalHiddenFieldAttribute, PropertyInfo>();
            this.fields = new ConcurrentDictionary<ModalFieldAttribute, PropertyInfo>();

            var properties = type.GetProperties().Where(x => x.GetSetMethod() != null);
            var hidden = properties.Where(x => x.GetCustomAttribute<ModalHiddenFieldAttribute>() != null);
            var visible = properties.Where(x => x.GetCustomAttribute<ModalFieldAttribute>() != null);

            foreach (var property in hidden)
                hiddenFields.TryAdd(property.GetCustomAttribute<ModalHiddenFieldAttribute>(), property);

            foreach (var property in visible)
                fields.TryAdd(property.GetCustomAttribute<ModalFieldAttribute>(), property);
        }

        public async Task CreateAsync(DiscordInteraction interaction, string title, IDictionary<string, string> hiddenValues)
        {
            var modalString = ExtensionStatics.GenerateIdString(Info.ModalId, hiddenValues);

            var interactionResp = new DiscordInteractionResponseBuilder()
                .WithTitle(title)
                .WithCustomId(modalString);

            foreach (var field in fields)
            {
                var attr = field.Key;
                interactionResp.AddComponents(
                    new TextInputComponent(attr.DisplayText, attr.FieldName,
                    attr.Placeholder, attr.Prefill, attr.Required, attr.InputStyle, attr.MinLength, attr.MaxLength));
            }

            await interaction.CreateResponseAsync(InteractionResponseType.Modal, interactionResp);
        }

        public async Task ExecuteAsync(ModalSubmitEventArgs e, IDictionary<string, string> hiddenValues, IServiceProvider services)
        {
            // Construct new modal with dependencies injected into constructor
            IEnumerable<object> constructorValues = Type.GetConstructors()[0].GetParameters().Select(x => services.GetService(x.ParameterType));
            var modal = (IModal)Activator.CreateInstance(Type, constructorValues.ToArray());

            // Inject hidden values into attributes
            if (hiddenValues != null)
            {
                foreach (var hidden in hiddenValues)
                {
                    hiddenFields.First(x => x.Key.FieldName == hidden.Key).Value.SetValue(modal, hidden.Value);
                }
            }

            // Inject visible modal fields
            foreach (var visible in e.Values)
            {
                fields.First(x => x.Key.FieldName == visible.Key).Value.SetValue(modal, visible.Value);
            }

            await modal.HandleAsync(e.Interaction);
        }
    }
}
