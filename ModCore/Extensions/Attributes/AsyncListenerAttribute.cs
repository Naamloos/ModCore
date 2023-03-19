using DSharpPlus;
using DSharpPlus.AsyncEvents;
using Microsoft.Extensions.Logging;
using ModCore.Extensions.Enums;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace ModCore.Extensions.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AsyncListenerAttribute : Attribute
    {
        public EventType Target { get; set; }

        public AsyncListenerAttribute(EventType target)
        {
            Target = target;
        }

        public void Register(DiscordClient client, MethodInfo listener, IServiceProvider services)
        {
            async Task onEvent(DiscordClient client, object e)
            {

                try
                {
                    List<object> parameters = new List<object>();
                    foreach (var param in listener.GetParameters())
                    {
                        if (param.ParameterType == typeof(DiscordClient))
                            parameters.Add(client);
                        else if (param.ParameterType.IsAssignableTo(typeof(AsyncEventArgs)))
                            parameters.Add(e);
                        else
                            parameters.Add(services.GetService(param.ParameterType));
                    }

                    await (Task)listener.Invoke(null, parameters.ToArray());
                }
                catch (Exception ex)
                {
                    client.Logger.LogError($"Uncaught error in event handler thread: {ex}");
                    client.Logger.LogError(ex.StackTrace);
                }
            };

            #region Registering correct events
            switch (Target)
            {
                case EventType.ClientErrored:
                    client.ClientErrored += onEvent;
                    break;
                case EventType.SocketErrored:
                    client.SocketErrored += onEvent;
                    break;
                case EventType.SocketOpened:
                    client.SocketOpened += onEvent;
                    break;
                case EventType.SocketClosed:
                    client.SocketClosed += onEvent;
                    break;
                case EventType.Ready:
                    client.Ready += onEvent;
                    break;
                case EventType.Resumed:
                    client.Resumed += onEvent;
                    break;
                case EventType.ChannelCreated:
                    client.ChannelCreated += onEvent;
                    break;
                case EventType.ChannelUpdated:
                    client.ChannelUpdated += onEvent;
                    break;
                case EventType.ChannelDeleted:
                    client.ChannelDeleted += onEvent;
                    break;
                case EventType.DmChannelDeleted:
                    client.DmChannelDeleted += onEvent;
                    break;
                case EventType.ChannelPinsUpdated:
                    client.ChannelPinsUpdated += onEvent;
                    break;
                case EventType.GuildCreated:
                    client.GuildCreated += onEvent;
                    break;
                case EventType.GuildAvailable:
                    client.GuildAvailable += onEvent;
                    break;
                case EventType.GuildUpdated:
                    client.GuildUpdated += onEvent;
                    break;
                case EventType.GuildDeleted:
                    client.GuildDeleted += onEvent;
                    break;
                case EventType.GuildUnavailable:
                    client.GuildUnavailable += onEvent;
                    break;
                case EventType.MessageCreated:
                    client.MessageCreated += onEvent;
                    break;
                case EventType.PresenceUpdated:
                    client.PresenceUpdated += onEvent;
                    break;
                case EventType.GuildBanAdded:
                    client.GuildBanAdded += onEvent;
                    break;
                case EventType.GuildBanRemoved:
                    client.GuildBanRemoved += onEvent;
                    break;
                case EventType.GuildEmojisUpdated:
                    client.GuildEmojisUpdated += onEvent;
                    break;
                case EventType.GuildIntegrationsUpdated:
                    client.GuildIntegrationsUpdated += onEvent;
                    break;
                case EventType.GuildMemberAdded:
                    client.GuildMemberAdded += onEvent;
                    break;
                case EventType.GuildMemberRemoved:
                    client.GuildMemberRemoved += onEvent;
                    break;
                case EventType.GuildMemberUpdated:
                    client.GuildMemberUpdated += onEvent;
                    break;
                case EventType.GuildRoleCreated:
                    client.GuildRoleCreated += onEvent;
                    break;
                case EventType.GuildRoleUpdated:
                    client.GuildRoleUpdated += onEvent;
                    break;
                case EventType.GuildRoleDeleted:
                    client.GuildRoleDeleted += onEvent;
                    break;
                case EventType.MessageAcknowledged:
                    client.MessageAcknowledged += onEvent;
                    break;
                case EventType.MessageUpdated:
                    client.MessageUpdated += onEvent;
                    break;
                case EventType.MessageDeleted:
                    client.MessageDeleted += onEvent;
                    break;
                case EventType.MessagesBulkDeleted:
                    client.MessagesBulkDeleted += onEvent;
                    break;
                case EventType.TypingStarted:
                    client.TypingStarted += onEvent;
                    break;
                case EventType.UserSettingsUpdated:
                    client.UserSettingsUpdated += onEvent;
                    break;
                case EventType.UserUpdated:
                    client.UserUpdated += onEvent;
                    break;
                case EventType.VoiceStateUpdated:
                    client.VoiceStateUpdated += onEvent;
                    break;
                case EventType.VoiceServerUpdated:
                    client.VoiceServerUpdated += onEvent;
                    break;
                case EventType.GuildMembersChunked:
                    client.GuildMembersChunked += onEvent;
                    break;
                case EventType.UnknownEvent:
                    client.UnknownEvent += onEvent;
                    break;
                case EventType.MessageReactionAdded:
                    client.MessageReactionAdded += onEvent;
                    break;
                case EventType.MessageReactionRemoved:
                    client.MessageReactionRemoved += onEvent;
                    break;
                case EventType.MessageReactionsCleared:
                    client.MessageReactionsCleared += onEvent;
                    break;
                case EventType.WebhooksUpdated:
                    client.WebhooksUpdated += onEvent;
                    break;
                case EventType.Heartbeated:
                    client.Heartbeated += onEvent;
                    break;
                case EventType.InviteCreate:
                    client.InviteCreated += onEvent;
                    break;
            }
            #endregion
        }
    }
}
