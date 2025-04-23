export interface ModCoreGuild {
  guild_id: string;
  modlog_channel_id?: string;
  ticket_channel_id?: string;
  appeal_channel_id?: string;
  nick_confirm_channel_id?: string;
  last_seen_at?: string;
  auto_role_enabled: boolean;
  embed_message_links_state: EmbedMessageLinks;
  persist_user_roles: boolean;
  persist_user_overrides: boolean;
  persist_user_nicknames: boolean;
}

export enum EmbedMessageLinks {
  Disabled = 0,
  Prefixed = 1,
  Always = 2,
}
