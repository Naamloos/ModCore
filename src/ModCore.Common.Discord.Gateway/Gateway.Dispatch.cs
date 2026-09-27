using Microsoft.Extensions.Logging;
using ModCore.Common.Discord.Gateway.EventData.Incoming;

namespace ModCore.Common.Discord.Gateway
{
    public partial class Gateway
    {
        private Task HandleDispatchAsync(Payload gatewayEvent)
        {
            switch (gatewayEvent.EventName)
            {
                case "READY":
                    lastReadyEvent = gatewayEvent.GetDataAs<Ready>(jsonSerializerOptions)!;
                    Application = lastReadyEvent.Application;
                    DispatchEventToSubscribers(lastReadyEvent);
                    break;
                case "RESUMED":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<Resumed>(jsonSerializerOptions)
                    );
                    break;
                case "RATE_LIMITED":
                    var limited = gatewayEvent.GetDataAs<RateLimited>(jsonSerializerOptions)!;
                    if (double.IsFinite(limited.RetryAfter) && limited.RetryAfter >= 0)
                        commandRetryTimes[
                            (limited.Opcode, limited.Meta?.GuildId?.ToString() ?? "")
                        ] = DateTimeOffset.UtcNow.AddSeconds(limited.RetryAfter);
                    DispatchEventToSubscribers(limited);
                    break;
                case "APPLICATION_COMMAND_PERMISSIONS_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<ApplicationCommandPermissionsUpdate>(
                            jsonSerializerOptions
                        )
                    );
                    break;
                case "AUTO_MODERATION_RULE_CREATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<AutoModerationRuleCreate>(jsonSerializerOptions)
                    );
                    break;
                case "AUTO_MODERATION_RULE_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<AutoModerationRuleUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "AUTO_MODERATION_RULE_DELETE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<AutoModerationRuleDelete>(jsonSerializerOptions)
                    );
                    break;
                case "AUTO_MODERATION_ACTION_EXECUTION":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<AutoModerationActionExecution>(jsonSerializerOptions)
                    );
                    break;
                case "CHANNEL_CREATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<ChannelCreate>(jsonSerializerOptions)
                    );
                    break;
                case "CHANNEL_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<ChannelUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "CHANNEL_DELETE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<ChannelDelete>(jsonSerializerOptions)
                    );
                    break;
                case "CHANNEL_INFO":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<ChannelInfo>(jsonSerializerOptions)
                    );
                    break;
                case "CHANNEL_PINS_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<ChannelPinsUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "THREAD_CREATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<ThreadCreate>(jsonSerializerOptions)
                    );
                    break;
                case "THREAD_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<ThreadUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "THREAD_DELETE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<ThreadDelete>(jsonSerializerOptions)
                    );
                    break;
                case "THREAD_LIST_SYNC":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<ThreadListSync>(jsonSerializerOptions)
                    );
                    break;
                case "THREAD_MEMBER_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<ThreadMemberUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "THREAD_MEMBERS_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<ThreadMembersUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "ENTITLEMENT_CREATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<EntitlementCreate>(jsonSerializerOptions)
                    );
                    break;
                case "ENTITLEMENT_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<EntitlementUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "ENTITLEMENT_DELETE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<EntitlementDelete>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_CREATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildCreate>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_DELETE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildDelete>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_AUDIT_LOG_ENTRY_CREATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildAuditLogEntryCreate>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_BAN_ADD":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildBanAdd>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_BAN_REMOVE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildBanRemove>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_EMOJIS_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildEmojisUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_STICKERS_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildStickersUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_INTEGRATIONS_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildIntegrationsUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_MEMBER_ADD":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildMemberAdd>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_MEMBER_REMOVE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildMemberRemove>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_MEMBER_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildMemberUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_MEMBERS_CHUNK":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildMembersChunk>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_ROLE_CREATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildRoleCreate>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_ROLE_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildRoleUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_ROLE_DELETE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildRoleDelete>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_SCHEDULED_EVENT_CREATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildScheduledEventCreate>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_SCHEDULED_EVENT_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildScheduledEventUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_SCHEDULED_EVENT_DELETE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildScheduledEventDelete>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_SCHEDULED_EVENT_USER_ADD":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildScheduledEventUserAdd>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_SCHEDULED_EVENT_USER_REMOVE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildScheduledEventUserRemove>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_SOUNDBOARD_SOUND_CREATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildSoundboardSoundCreate>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_SOUNDBOARD_SOUND_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildSoundboardSoundUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_SOUNDBOARD_SOUND_DELETE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildSoundboardSoundDelete>(jsonSerializerOptions)
                    );
                    break;
                case "GUILD_SOUNDBOARD_SOUNDS_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<GuildSoundboardSoundsUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "SOUNDBOARD_SOUNDS":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<SoundboardSounds>(jsonSerializerOptions)
                    );
                    break;
                case "INTEGRATION_CREATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<IntegrationCreate>(jsonSerializerOptions)
                    );
                    break;
                case "INTEGRATION_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<IntegrationUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "INTEGRATION_DELETE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<IntegrationDelete>(jsonSerializerOptions)
                    );
                    break;
                case "INTERACTION_CREATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<InteractionCreate>(jsonSerializerOptions)
                    );
                    break;
                case "INVITE_CREATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<InviteCreate>(jsonSerializerOptions)
                    );
                    break;
                case "INVITE_DELETE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<InviteDelete>(jsonSerializerOptions)
                    );
                    break;
                case "MESSAGE_CREATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<MessageCreate>(jsonSerializerOptions)
                    );
                    break;
                case "MESSAGE_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<MessageUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "MESSAGE_DELETE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<MessageDelete>(jsonSerializerOptions)
                    );
                    break;
                case "MESSAGE_DELETE_BULK":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<MessageBulkDelete>(jsonSerializerOptions)
                    );
                    break;
                case "MESSAGE_REACTION_ADD":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<MessageReactionAdd>(jsonSerializerOptions)
                    );
                    break;
                case "MESSAGE_REACTION_REMOVE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<MessageReactionRemove>(jsonSerializerOptions)
                    );
                    break;
                case "MESSAGE_REACTION_REMOVE_ALL":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<MessageReactionRemoveAll>(jsonSerializerOptions)
                    );
                    break;
                case "MESSAGE_REACTION_REMOVE_EMOJI":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<MessageReactionRemoveEmoji>(jsonSerializerOptions)
                    );
                    break;
                case "PRESENCE_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<PresenceUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "STAGE_INSTANCE_CREATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<StageInstanceCreate>(jsonSerializerOptions)
                    );
                    break;
                case "STAGE_INSTANCE_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<StageInstanceUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "STAGE_INSTANCE_DELETE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<StageInstanceDelete>(jsonSerializerOptions)
                    );
                    break;
                case "SUBSCRIPTION_CREATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<SubscriptionCreate>(jsonSerializerOptions)
                    );
                    break;
                case "SUBSCRIPTION_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<SubscriptionUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "SUBSCRIPTION_DELETE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<SubscriptionDelete>(jsonSerializerOptions)
                    );
                    break;
                case "TYPING_START":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<TypingStart>(jsonSerializerOptions)
                    );
                    break;
                case "USER_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<UserUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "VOICE_CHANNEL_EFFECT_SEND":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<VoiceChannelEffectSend>(jsonSerializerOptions)
                    );
                    break;
                case "VOICE_CHANNEL_START_TIME_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<VoiceChannelStartTimeUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "VOICE_CHANNEL_STATUS_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<VoiceChannelStatusUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "VOICE_STATE_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<VoiceStateUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "VOICE_SERVER_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<VoiceServerUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "WEBHOOKS_UPDATE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<WebhooksUpdate>(jsonSerializerOptions)
                    );
                    break;
                case "MESSAGE_POLL_VOTE_ADD":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<MessagePollVoteAdd>(jsonSerializerOptions)
                    );
                    break;
                case "MESSAGE_POLL_VOTE_REMOVE":
                    DispatchEventToSubscribers(
                        gatewayEvent.GetDataAs<MessagePollVoteRemove>(jsonSerializerOptions)
                    );
                    break;
                default:
                    DispatchEventToSubscribers(
                        new UnknownDispatch
                        {
                            Name = gatewayEvent.EventName ?? "",
                            Data = gatewayEvent.Data.Clone(),
                            Sequence = gatewayEvent.Sequence,
                        }
                    );
                    break;
            }
            return Task.CompletedTask;
        }
    }
}
