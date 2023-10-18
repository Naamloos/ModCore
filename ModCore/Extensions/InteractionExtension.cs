using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using ModCore.Extensions.Abstractions;
using ModCore.Extensions.Attributes;
using ModCore.Extensions.Handlers;
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
        private readonly ConcurrentBag<ModalHandler> modalHandlers;
        private readonly ConcurrentBag<ComponentHandler> componentHandlers;
        private DiscordClient client;
        private ILogger logger;

        public IServiceProvider Services { get => services; }
        private readonly IServiceProvider services;

        public InteractionExtension(IServiceProvider services)
        {
            this.services = services;
            this.modalHandlers = new ConcurrentBag<ModalHandler>();
            this.componentHandlers = new ConcurrentBag<ComponentHandler>();
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

        public string GenerateId(string id, params (string Key, string Value)[] values)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            foreach (var value in values)
            {
                keyValuePairs.Add(value.Key, value.Value);
            }

            return GenerateIdWithValues(id, keyValuePairs);
        }

        public string GenerateIdWithValues(string id, IDictionary<string, string> values)
        {
            return ExtensionStatics.GenerateIdString(id, values);
        }

        protected override void Setup(DiscordClient client)
        {
            this.client = client;

            setupModals(client);
            setupComponents(client);
        }

        private void setupComponents(DiscordClient client)
        {
            var handlers = this.GetType().Assembly.GetTypes().Where(x => x.IsAssignableTo(typeof(BaseComponentModule)) && !x.IsAbstract);
            foreach(var handler in handlers)
            {
                registerComponentHandler(handler);
            }

            client.ComponentInteractionCreated += (sender, e) =>
            {
                _ = Task.Run(async () => await handleComponentAsync(sender, e));
                return Task.CompletedTask;
            };
        }

        private void registerComponentHandler(Type type)
        {
            componentHandlers.Add(new ComponentHandler(type, services, this.client));
        }

        private async Task handleComponentAsync(DiscordClient sender, ComponentInteractionCreateEventArgs e)
        {
            var deciphered = ExtensionStatics.DecipherIdString(e.Interaction.Data.CustomId);

            foreach(var handler in componentHandlers)
            {
                if (!handler.HasHandlerFor(deciphered.Id, e.Interaction.Data.ComponentType))
                    continue;

                try
                {
                    await handler.HandleAsync(e, deciphered.Id, deciphered.Values);
                }catch(Exception ex)
                {
                    this.logger.LogError(new EventId(6969, "Interactions"), ex, $"Component with id {deciphered.Id} errored!");
                }
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

        public override void Dispose()
        {

        }
    }
}
