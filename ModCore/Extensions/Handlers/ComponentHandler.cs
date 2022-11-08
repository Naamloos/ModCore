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
    public class ComponentHandler
    {
        private ConcurrentDictionary<ComponentAttribute, MethodInfo> handlerMethods;
        private BaseComponentModule instance;
        private Permissions requiredPermissions;

        public ComponentHandler(Type type, IServiceProvider services, DiscordClient client)
        {
            handlerMethods = new ConcurrentDictionary<ComponentAttribute, MethodInfo>();

            var methods = type.GetMethods().Where(x => x.GetCustomAttribute<ComponentAttribute>() != null).ToList();
            var validMethods = methods.Where(x => 
            {
                if (x.ReturnType != typeof(Task))
                    return false;

                var parameters = x.GetParameters();

                if(parameters.Length == 2)
                {
                    return parameters[0].ParameterType == typeof(ComponentInteractionCreateEventArgs)
                        && parameters[1].ParameterType == typeof(IDictionary<string, string>);
                }

                return parameters.Length == 1 
                    && parameters[0].ParameterType == typeof(ComponentInteractionCreateEventArgs);
            });

            foreach(var valid in validMethods)
            {
                handlerMethods.TryAdd(valid.GetCustomAttribute<ComponentAttribute>(), valid);
            }

            var constructor = type.GetConstructors()[0];
            var constructorParameters = constructor.GetParameters();
            var parameters = new object[constructorParameters.Length];
            for(var i = 0; i < constructorParameters.Length; i++)
            {
                parameters[i] = services.GetService(constructorParameters[i].ParameterType);
            }

            var perms = type.GetCustomAttribute<ComponentPermissionsAttribute>();
            if(perms != null)
            {
                this.requiredPermissions = perms.Permissions;
            }

            instance = (BaseComponentModule)Activator.CreateInstance(type, parameters);
            instance.Client = client;
        }

        public bool HasHandlerFor(string id, ComponentType type)
        {
            return handlerMethods.Any(x => x.Key.Id == id && x.Key.ComponentType == type);
        }

        public async Task HandleAsync(ComponentInteractionCreateEventArgs e, string command, IDictionary<string, string> commandArgs)
        {
            if (!e.Channel.PermissionsFor(e.User as DiscordMember).HasPermission(requiredPermissions))
                return;

            var handlerKey = this.handlerMethods.Keys.FirstOrDefault(x => x.Id == command && x.ComponentType == e.Interaction.Data.ComponentType);
            if (handlerKey == null)
                return;
            var handler = this.handlerMethods[handlerKey];

            var perms = handler.GetCustomAttribute<ComponentPermissionsAttribute>();
            if (perms != null)
            {
                if (!e.Channel.PermissionsFor(e.User as DiscordMember).HasPermission(perms.Permissions))
                    return;
            }

            if (handler.GetParameters().Length == 2)
                await (Task)(handler.Invoke(instance, new object[] { e, commandArgs }));
            await (Task)(handler.Invoke(instance, new object[] { e }));
        }
    }
}
