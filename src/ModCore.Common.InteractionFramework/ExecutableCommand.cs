using Microsoft.Extensions.DependencyInjection;
using ModCore.Common.InteractionFramework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ModCore.Common.InteractionFramework
{
    public class ExecutableCommand 
    {
        private BaseCommandHandler _handler;
        private MethodInfo _command;

        private Dictionary<string, ParameterInfo> _parameters;

        public ExecutableCommand(BaseCommandHandler handler, MethodInfo command)
        {
            this._command = command;
            this._handler = handler;
            this._parameters = new Dictionary<string, ParameterInfo>();

            foreach(var param in command.GetParameters().Skip(1))
            {
                var optionAttribute = param.GetCustomAttribute<OptionAttribute>();
                if(optionAttribute is null)
                {
                    throw new InvalidOperationException("All parameters must have an OptionAttribute! " +
                        $"Missing: {param.Name} in method {_command.Name} for handler {_handler.GetType().Name}");
                }

                _parameters.Add(optionAttribute.Name, param);
            }
        }

        public async ValueTask Execute(SlashCommandContext context)
        {
            List<object> parameters = [context];

            foreach (var param in _parameters)
            {
                var option = context.OptionValues.FirstOrDefault(x => x.Name.ToLowerInvariant() == param.Key);
                if (option is null || !option.Value.HasValue)
                {
                    parameters.Add(null!);
                }
                else
                {
                    parameters.Add(JsonSerializer.Deserialize(option.Value.Value, param.Value.ParameterType, options: context.ServiceProvider.GetService<JsonSerializerOptions>())!);
                }
            }

            await (ValueTask)_command.Invoke(_handler, parameters.ToArray())!;
        }
    }
}
