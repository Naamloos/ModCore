using ModCore.Common.Discord.Gateway.EventData.Incoming;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Services.Shard.Interactions
{
    public class CommandMap
    {
        private ConcurrentDictionary<string, RegisteredCommand> commands;

        public CommandMap()
        {
            this.commands = new ConcurrentDictionary<string, RegisteredCommand>();
        }

        public void Register(string fullName, object handlerObject, MethodInfo? resolvedMethod)
        {
            if(resolvedMethod == null || handlerObject == null)
            {
                return; // no-op so ignore
            }

            commands.TryAdd(fullName, new RegisteredCommand()
            {
                HandlerObject = handlerObject,
                Method = resolvedMethod,
            });
        }

        public Task InvokeAsync(string fullName, InteractionCreate data)
        {
            if (commands.TryGetValue(fullName, out RegisteredCommand command))
            {
                return command.Method.Invoke(command.HandlerObject, new object[] {data}) as Task;
            }

            return Task.CompletedTask;
        }

        private struct RegisteredCommand
        {
            public object HandlerObject;
            public MethodInfo Method;
        }
    }
}
