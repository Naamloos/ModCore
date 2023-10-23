using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
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
                var parameters = command.Method.GetParameters();
                var resolvedParameters = new Dictionary<string, object>();
                var depth = fullName.Split(' ').Length;

                List<ApplicationCommandInteractionDataOption> options;
                if (depth == 1)
                    options = data.Data.Value.options.Value;
                if (depth == 2)
                    options = data.Data.Value.options.Value[0].Options;
                else // or the loop breks
                    options = data.Data.Value.options.Value[0].Options.Value[0].Options;

                foreach(var option in options)
                {
                    resolvedParameters.Add(option.Name, option.Type switch
                    {
                        ApplicationCommandOptionType.String => option.Value.Value.GetValue<string>(),
                        ApplicationCommandOptionType.Integer => option.Value.Value.GetValue<int>(),
                        ApplicationCommandOptionType.Number => option.Value.Value.GetValue<double>(),
                        ApplicationCommandOptionType.User => option.Value.Value.GetValue<Snowflake>(),
                        _ => throw new Exception("unimplemented option type, oops")
                    });
                }

                List<object> allparameters = new List<object>();
                allparameters.Add(data);
                allparameters.AddRange(command.Method.GetParameters().Skip(1).Select(x => resolvedParameters[x.Name]));

                return command.Method.Invoke(command.HandlerObject, allparameters.ToArray()) as Task;
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
