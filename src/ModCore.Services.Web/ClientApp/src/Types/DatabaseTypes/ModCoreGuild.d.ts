import { ModCoreAutorole } from "./ModCoreAutorole";
import { ModCoreLoggerSettings } from "./ModCoreLoggerSettings";
import { ModCoreRoleMenu } from "./ModCoreRoleMenu";
import { ModCoreStarboard } from "./ModCoreStarboard";
import { ModCoreWelcomeSettings } from "./ModCoreWelcomeSettings";

export type ModCoreGuild =
{
    logging_channel_id: bigint | null;
    modlog_channel_id: bigint | null;
    ticket_channel_id: bigint | null;
    appeal_channel_id: bigint | null;
    nick_confirm_channel_id: bigint | null;
    last_seen_at: Date | null;
    auto_role_enabled: boolean;
    embed_message_links_state: EmbedMessageLinks;
    persist_user_roles: boolean;
    persist_user_nicknames: boolean;
    persist_user_overrides: boolean;
    starboards: ModCoreStarboard[];
    auto_roles: ModCoreAutorole[];
    role_menus: ModCoreRoleMenu[];
    logger_settings: ModCoreLoggerSettings;
    welcome_settings: ModCoreWelcomeSettings;
    level_settings: ModCoreLevelSettings;
}

export enum EmbedMessageLinks
{
    Disabled = 0,
    Prefixed = 1,
    Always = 2
}