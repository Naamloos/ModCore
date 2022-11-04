using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Extensions.Attributes;
using ModCore.Extensions.Handlers;
using ModCore.Extensions.Interfaces;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ModCore.Extensions
{
    public class InteractionExtension : BaseExtension
    {
        private readonly ConcurrentBag<ButtonHandler> buttonHandlers;
        private readonly ConcurrentBag<ModalHandler> modalHandlers;

        private readonly IServiceProvider services;

        public InteractionExtension(IServiceProvider services)
        {
            this.services = services;
            this.buttonHandlers = new ConcurrentBag<ButtonHandler>();
            this.modalHandlers = new ConcurrentBag<ModalHandler>();
        }

        public string GenerateButton<T>(params (string Key, string Value)[] values) where T : IButton
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            foreach (var value in values)
            {
                keyValuePairs.Add(value.Key, value.Value);
            }

            return GenerateButton<T>(keyValuePairs);
        }

        public string GenerateButton<T>(IDictionary<string, string> values = null) where T : IButton
        {
            var handler = buttonHandlers.FirstOrDefault(x => x.HandlerType == typeof(T));
            if (handler == null)
            {
                // Cache for next call :^)
                handler = registerButtonHandler(typeof(T));
            }

            return handler.Create(values);
        }

        public async Task RespondWithModalAsync<T>(DiscordInteraction interaction, string title,
            IDictionary<string, string> hiddenValues = null) where T : IModal
        {
            var handler = modalHandlers.FirstOrDefault(x => x.Type == typeof(T));
            if (handler == null)
            {
                // Cache for next call :^)
                handler = registerModalHandler(typeof(T));
            }

            await handler.CreateAsync(interaction, title, hiddenValues);
        }

        protected override void Setup(DiscordClient client)
        {
            setupButtons(client);
            setupModals(client);
        }

        private void setupButtons(DiscordClient client)
        {
            var handlers = this.GetType().Assembly.GetTypes().Where(x => x.IsAssignableTo(typeof(IButton)) && !x.IsInterface);
            foreach (var handler in handlers)
            {
                registerButtonHandler(handler);
            }

            client.ComponentInteractionCreated += (sender, e) =>
            {
                _ = Task.Run(async () => await handleButtonAsync(sender, e));
                return Task.CompletedTask;
            };
        }

        private ButtonHandler registerButtonHandler(Type type)
        {
            var buttonAttributes = type.GetCustomAttributes(typeof(ButtonAttribute), false);
            if (buttonAttributes.Length != 1)
            {
                throw new Exception($"{type} has no {typeof(ButtonAttribute)}!");
            }

            var buttonAttribute = buttonAttributes[0] as ButtonAttribute;

            if (type.GetConstructors().Length != 1)
                throw new Exception($"{type} has more than 1 constructor!");

            if (buttonHandlers.Any(x => x.HandlerType == type))
                throw new Exception($"Modal with type {type} already registered!");

            var handler = new ButtonHandler(buttonAttribute, type);
            buttonHandlers.Add(handler);

            return handler;
        }

        private async Task handleButtonAsync(DiscordClient sender, ComponentInteractionCreateEventArgs e)
        {
            if (e.Interaction.Data.ComponentType == ComponentType.Button)
            {
                var deciphered = ExtensionStatics.DecipherIdString(e.Interaction.Data.CustomId);

                var handler = buttonHandlers.FirstOrDefault(x => x.Data.Id == deciphered.Id);
                if (handler != null)
                    await handler.ExecuteAsync(e, deciphered.Values, services);
            }
        }

        private void setupModals(DiscordClient client)
        {
            var handlers = this.GetType().Assembly.GetTypes().Where(x => x.IsAssignableTo(typeof(IModal)) && !x.IsInterface);
            foreach (var handler in handlers)
            {
                registerModalHandler(handler);
            }

            client.ModalSubmitted += (sender, e) =>
            {
                _ = Task.Run(async () => await handleModalSubmission(sender, e));
                return Task.CompletedTask;
            };
        }

        private ModalHandler registerModalHandler(Type type)
        {
            var modalAttributes = type.GetCustomAttributes(typeof(ModalAttribute), false);
            if (modalAttributes.Length != 1)
            {
                throw new Exception($"{type} has no {typeof(ModalAttribute)}!");
            }

            var modalAttribute = modalAttributes[0] as ModalAttribute;

            if (type.GetConstructors().Length != 1)
                throw new Exception($"{type} has more than 1 constructor!");

            if (modalHandlers.Any(x => x.Type == type))
                throw new Exception($"Modal with type {type} already registered!");

            var handler = new ModalHandler(type, modalAttribute);
            modalHandlers.Add(handler);

            return handler;
        }

        private async Task handleModalSubmission(DiscordClient sender, ModalSubmitEventArgs e)
        {
            var deciphered = ExtensionStatics.DecipherIdString(e.Interaction.Data.CustomId);

            var handler = modalHandlers.FirstOrDefault(x => x.Info.ModalId == deciphered.Id);
            if (handler != null)
                await handler.ExecuteAsync(e, deciphered.Values, services);
        }
    }
}
