using Microsoft.Extensions.Logging;
using ModCore.Common.Discord.Gateway;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Gateway.Events;
using ModCore.Common.Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Interactions;
using System.Text.RegularExpressions;
using ModCore.Common.Discord.Entities.Messages;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using ModCore.Common.Utils;
using System.Diagnostics;

namespace ModCore.Services.Shard.EventHandlers
{
    public class SimpleEvalEvent : ISubscriber<MessageCreate>
    {
        public Gateway Gateway { get; set; }

        private readonly ILogger _logger;
        private readonly DiscordRest _api;
        private readonly Gateway _gateway;
        private User _modCore;
        private Application _app;

        public SimpleEvalEvent(ILogger<SimpleEvalEvent> logger, DiscordRest api, Gateway gateway) 
        {
            this._logger = logger;
            this._api = api;
            this._gateway = gateway;
        }

        private Regex codeRegex = new Regex(@"```c?s?((.|\n)*?)```", RegexOptions.Compiled);
        public async ValueTask HandleEvent(MessageCreate data)
        {
            if(_modCore == default)
            {
                var getUser = await _api.GetCurrentUserAsync();
                if(!getUser.Success)
                {
                    return;
                }
                _modCore = getUser.Value!;
            }

            if(_app == default)
            {
                var getApp = await _api.GetApplicationAsync(_gateway.Application.Id);
                if(!getApp.Success)
                {
                    return;
                }
                _app = getApp.Value!;
            }

            if(data.Mentions.Any(x => x.Id == _modCore.Id) && _app.Owner.Value?.Id == data.Author.Id)
            {
                var initialMessage = await _api.CreateMessageAsync(data.ChannelId, new CreateMessage()
                {
                    Content = $"Evaluating code block... <t:{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}:R>",
                    MessageReference = new MessageReference()
                    {
                        ChannelId = data.ChannelId,
                        FailIfNotExists = false,
                        GuildId = data.GuildId,
                        MessageId = data.Id
                    }
                });

                if(!initialMessage.Success)
                {
                    return;
                }

                // This is the app owner trying to perform magic.
                var match = codeRegex.Match(data.Content);
                if (!match.Success)
                {
                    return;
                }

                _logger.LogDebug(match.Groups[1].Value);
                var resultEmbed = await executeAsync(match.Groups[1].Value, data);

                await _api.ModifyMessageAsync(data.ChannelId, initialMessage.Value.Id, new CreateMessage()
                {
                    Content = "",
                    Embeds = new Embed[] { resultEmbed }
                });
            }
        }

        private async ValueTask<Embed> executeAsync(string inputCode, MessageCreate context)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var resultEmbed = new Embed()
                .WithTitle("Evaluation Results");

            var variables = new EvalVariables(_gateway, _api, context);
            var options = ScriptOptions.Default
                .WithImports("System", 
                    "System.Collections.Generic", 
                    "System.Linq", 
                    "System.Text", 
                    "System.Threading.Tasks", 
                    "ModCore.Common.Discord.Gateway", 
                    "ModCore.Common.Discord.Rest", 
                    "ModCore.Common.Discord.Entities")
                .WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(xa => !xa.IsDynamic && !string.IsNullOrWhiteSpace(xa.Location)));

            try
            {
                var script = CSharpScript.Create(inputCode, options, typeof(EvalVariables));
                script.Compile();
                var results = await script.RunAsync(variables);

                resultEmbed.WithDescription($"✅ Evaluation successful.");
                resultEmbed.WithColor(ColorConverter.FromHex("#00b506"));

                if(string.IsNullOrEmpty(results?.ReturnValue?.ToString()))
                {
                    resultEmbed.WithField("Return value", "Empty");
                }
                else
                {
                    resultEmbed.WithField("Return Type", $"`{results.ReturnValue.GetType().ToString().Replace("`", "'")}`");
                    resultEmbed.WithField("Return Value", results.ReturnValue.ToString());
                }
            }
            catch(Exception ex)
            {
                resultEmbed.WithColor(ColorConverter.FromHex("#910101"));
                resultEmbed.WithDescription($"⚠️ Exception thrown: `{ex.Message}`");
            }
            finally
            {
                stopwatch.Stop();
            }

            resultEmbed.WithFooter($"Evaluation time: {stopwatch.Elapsed.ToString()}");

            return resultEmbed;
        }

        public class EvalVariables
        {
            public Gateway Gateway { get; set; }
            public DiscordRest Rest { get; set; }
            public MessageCreate Context { get; set; }

            public EvalVariables(Gateway gateway, DiscordRest rest, MessageCreate context)
            {
                this.Gateway = gateway;
                this.Rest = rest;
                this.Context = context;
            }
        }
    }
}
