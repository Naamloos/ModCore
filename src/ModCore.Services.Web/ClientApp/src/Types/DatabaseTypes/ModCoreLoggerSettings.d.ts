export default interface ModCoreLoggerSettings {
    guild_id: string;
    logger_channel_id?: string;
    log_joins: boolean;
    log_message_edit: boolean;
    log_nicknames: boolean;
    log_avatars: boolean;
    log_invites: boolean;
    log_role_assign: boolean;
    log_channels: boolean;
    log_guild_edit: boolean;
    log_role_edit: boolean;
}