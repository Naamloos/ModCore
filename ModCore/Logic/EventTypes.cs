namespace ModCore.Logic
{
    // CNext doesn't need all the fiddling, so we dont need those events.
    public enum EventTypes
    {
        ClientErrored, // AsyncEventHandler<ClientErrorEventArgs>
        SocketErrored, // AsyncEventHandler<SocketErrorEventArgs>
        SocketOpened, // AsyncEventHandler<Void>
        SocketClosed, // AsyncEventHandler<SocketCloseEventArgs>
        Ready, // AsyncEventHandler<ReadyEventArgs>
        Resumed, // AsyncEventHandler<ReadyEventArgs>
        ChannelCreated, // AsyncEventHandler<ChannelCreateEventArgs>
        DmChannelCreated, // AsyncEventHandler<DmChannelCreateEventArgs>
        ChannelUpdated, // AsyncEventHandler<ChannelUpdateEventArgs>
        ChannelDeleted, // AsyncEventHandler<ChannelDeleteEventArgs>
        DmChannelDeleted, // AsyncEventHandler<DmChannelDeleteEventArgs>
        ChannelPinsUpdated, // AsyncEventHandler<ChannelPinsUpdateEventArgs>
        GuildCreated, // AsyncEventHandler<GuildCreateEventArgs>
        GuildAvailable, // AsyncEventHandler<GuildCreateEventArgs>
        GuildUpdated, // AsyncEventHandler<GuildUpdateEventArgs>
        GuildDeleted, // AsyncEventHandler<GuildDeleteEventArgs>
        GuildUnavailable, // AsyncEventHandler<GuildDeleteEventArgs>
        MessageCreated, // AsyncEventHandler<MessageCreateEventArgs>
        PresenceUpdated, // AsyncEventHandler<PresenceUpdateEventArgs>
        GuildBanAdded, // AsyncEventHandler<GuildBanAddEventArgs>
        GuildBanRemoved, // AsyncEventHandler<GuildBanRemoveEventArgs>
        GuildEmojisUpdated, // AsyncEventHandler<GuildEmojisUpdateEventArgs>
        GuildIntegrationsUpdated, // AsyncEventHandler<GuildIntegrationsUpdateEventArgs>
        GuildMemberAdded, // AsyncEventHandler<GuildMemberAddEventArgs>
        GuildMemberRemoved, // AsyncEventHandler<GuildMemberRemoveEventArgs>
        GuildMemberUpdated, // AsyncEventHandler<GuildMemberUpdateEventArgs>
        GuildRoleCreated, // AsyncEventHandler<GuildRoleCreateEventArgs>
        GuildRoleUpdated, // AsyncEventHandler<GuildRoleUpdateEventArgs>
        GuildRoleDeleted, // AsyncEventHandler<GuildRoleDeleteEventArgs>
        MessageAcknowledged, // AsyncEventHandler<MessageAcknowledgeEventArgs>
        MessageUpdated, // AsyncEventHandler<MessageUpdateEventArgs>
        MessageDeleted, // AsyncEventHandler<MessageDeleteEventArgs>
        MessagesBulkDeleted, // AsyncEventHandler<MessageBulkDeleteEventArgs>
        TypingStarted, // AsyncEventHandler<TypingStartEventArgs>
        UserSettingsUpdated, // AsyncEventHandler<UserSettingsUpdateEventArgs>
        UserUpdated, // AsyncEventHandler<UserUpdateEventArgs>
        VoiceStateUpdated, // AsyncEventHandler<VoiceStateUpdateEventArgs>
        VoiceServerUpdated, // AsyncEventHandler<VoiceServerUpdateEventArgs>
        GuildMembersChunked, // AsyncEventHandler<GuildMembersChunkEventArgs>
        UnknownEvent, // AsyncEventHandler<UnknownEventArgs>
        MessageReactionAdded, // AsyncEventHandler<MessageReactionAddEventArgs>
        MessageReactionRemoved, // AsyncEventHandler<MessageReactionRemoveEventArgs>
        MessageReactionsCleared, // AsyncEventHandler<MessageReactionsClearEventArgs>
        WebhooksUpdated, // AsyncEventHandler<WebhooksUpdateEventArgs>
        Heartbeated, // AsyncEventHandler<HeartbeatEventArgs>
        CommandExecuted,
        CommandErrored,
        InviteCreate
    }
}