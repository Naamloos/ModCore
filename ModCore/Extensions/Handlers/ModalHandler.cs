using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Extensions.Abstractions;
using ModCore.Extensions.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ModCore.Extensions.Handlers
{
    public class ModalHandler
    {
        public Type Type { get; private set; }

        public ModalAttribute Info { get; private set; }

        private readonly ConcurrentDictionary<ModalFieldAttribute, PropertyInfo> fields;

        private readonly ConcurrentDictionary<ModalHiddenFieldAttribute, PropertyInfo> hiddenFields;

        public ModalHandler(Type type, ModalAttribute info)
        {
            Type = type;
            Info = info;
            hiddenFields = new ConcurrentDictionary<ModalHiddenFieldAttribute, PropertyInfo>();
            fields = new ConcurrentDictionary<ModalFieldAttribute, PropertyInfo>();

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
