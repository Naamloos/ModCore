using System;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;

namespace ModCore.Logic
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AsyncListenerAttribute : Attribute
    {
        public EventTypes Target { get; }

        public AsyncListenerAttribute(EventTypes targetType)
        {
            Target = targetType;
        }

        public void Register(ModCoreShard bot, DiscordClient client, MethodInfo listener)
        {
            // TODO redo this with linq expressions
            // nope, there's no cleaner way to do this. sorry
            Task OnEventWithArgs(DiscordClient c, object e)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await (Task) listener.Invoke(null, new[] {bot, e});
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"<AsyncListener> Uncaught exception in listener thread: {ex}");
                        Console.WriteLine(ex.StackTrace);
                    }
                });
                return Task.CompletedTask;
            }

            Task OnCnextEvent(CommandsNextExtension cmd, object e)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await (Task)listener.Invoke(null, new[] { bot, e });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"<AsyncListener> Uncaught exception in listener thread: {ex}");
                        Console.WriteLine(ex.StackTrace);
                    }
                });
                return Task.CompletedTask;
            }

            switch (Target)
            {
                case EventTypes.ClientErrored:
                    client.ClientErrored += OnEventWithArgs;
                    break;
                case EventTypes.SocketErrored:
                    client.SocketErrored += OnEventWithArgs;
                    break;
                case EventTypes.SocketOpened:
                    client.SocketOpened += OnEventWithArgs;
                    break;
                case EventTypes.SocketClosed:
                    client.SocketClosed += OnEventWithArgs;
                    break;
                case EventTypes.Ready:
                    client.Ready += OnEventWithArgs;
                    break;
                case EventTypes.Resumed:
                    client.Resumed += OnEventWithArgs;
                    break;
                case EventTypes.ChannelCreated:
                    client.ChannelCreated += OnEventWithArgs;
                    break;
                case EventTypes.DmChannelCreated:
                    client.DmChannelCreated += OnEventWithArgs;
                    break;
                case EventTypes.ChannelUpdated:
                    client.ChannelUpdated += OnEventWithArgs;
                    break;
                case EventTypes.ChannelDeleted:
                    client.ChannelDeleted += OnEventWithArgs;
                    break;
                case EventTypes.DmChannelDeleted:
                    client.DmChannelDeleted += OnEventWithArgs;
                    break;
                case EventTypes.ChannelPinsUpdated:
                    client.ChannelPinsUpdated += OnEventWithArgs;
                    break;
                case EventTypes.GuildCreated:
                    client.GuildCreated += OnEventWithArgs;
                    break;
                case EventTypes.GuildAvailable:
                    client.GuildAvailable += OnEventWithArgs;
                    break;
                case EventTypes.GuildUpdated:
                    client.GuildUpdated += OnEventWithArgs;
                    break;
                case EventTypes.GuildDeleted:
                    client.GuildDeleted += OnEventWithArgs;
                    break;
                case EventTypes.GuildUnavailable:
                    client.GuildUnavailable += OnEventWithArgs;
                    break;
                case EventTypes.MessageCreated:
                    client.MessageCreated += OnEventWithArgs;
                    break;
                case EventTypes.PresenceUpdated:
                    client.PresenceUpdated += OnEventWithArgs;
                    break;
                case EventTypes.GuildBanAdded:
                    client.GuildBanAdded += OnEventWithArgs;
                    break;
                case EventTypes.GuildBanRemoved:
                    client.GuildBanRemoved += OnEventWithArgs;
                    break;
                case EventTypes.GuildEmojisUpdated:
                    client.GuildEmojisUpdated += OnEventWithArgs;
                    break;
                case EventTypes.GuildIntegrationsUpdated:
                    client.GuildIntegrationsUpdated += OnEventWithArgs;
                    break;
                case EventTypes.GuildMemberAdded:
                    client.GuildMemberAdded += OnEventWithArgs;
                    break;
                case EventTypes.GuildMemberRemoved:
                    client.GuildMemberRemoved += OnEventWithArgs;
                    break;
                case EventTypes.GuildMemberUpdated:
                    client.GuildMemberUpdated += OnEventWithArgs;
                    break;
                case EventTypes.GuildRoleCreated:
                    client.GuildRoleCreated += OnEventWithArgs;
                    break;
                case EventTypes.GuildRoleUpdated:
                    client.GuildRoleUpdated += OnEventWithArgs;
                    break;
                case EventTypes.GuildRoleDeleted:
                    client.GuildRoleDeleted += OnEventWithArgs;
                    break;
                case EventTypes.MessageAcknowledged:
                    client.MessageAcknowledged += OnEventWithArgs;
                    break;
                case EventTypes.MessageUpdated:
                    client.MessageUpdated += OnEventWithArgs;
                    break;
                case EventTypes.MessageDeleted:
                    client.MessageDeleted += OnEventWithArgs;
                    break;
                case EventTypes.MessagesBulkDeleted:
                    client.MessagesBulkDeleted += OnEventWithArgs;
                    break;
                case EventTypes.TypingStarted:
                    client.TypingStarted += OnEventWithArgs;
                    break;
                case EventTypes.UserSettingsUpdated:
                    client.UserSettingsUpdated += OnEventWithArgs;
                    break;
                case EventTypes.UserUpdated:
                    client.UserUpdated += OnEventWithArgs;
                    break;
                case EventTypes.VoiceStateUpdated:
                    client.VoiceStateUpdated += OnEventWithArgs;
                    break;
                case EventTypes.VoiceServerUpdated:
                    client.VoiceServerUpdated += OnEventWithArgs;
                    break;
                case EventTypes.GuildMembersChunked:
                    client.GuildMembersChunked += OnEventWithArgs;
                    break;
                case EventTypes.UnknownEvent:
                    client.UnknownEvent += OnEventWithArgs;
                    break;
                case EventTypes.MessageReactionAdded:
                    client.MessageReactionAdded += OnEventWithArgs;
                    break;
                case EventTypes.MessageReactionRemoved:
                    client.MessageReactionRemoved += OnEventWithArgs;
                    break;
                case EventTypes.MessageReactionsCleared:
                    client.MessageReactionsCleared += OnEventWithArgs;
                    break;
                case EventTypes.WebhooksUpdated:
                    client.WebhooksUpdated += OnEventWithArgs;
                    break;
                case EventTypes.Heartbeated:
                    client.Heartbeated += OnEventWithArgs;
                    break;
                case EventTypes.CommandExecuted:
                    bot.Commands.CommandExecuted += OnCnextEvent;
                    break;
                case EventTypes.CommandErrored:
                    bot.Commands.CommandErrored += OnCnextEvent;
                    break;
            }
        }
    }
}