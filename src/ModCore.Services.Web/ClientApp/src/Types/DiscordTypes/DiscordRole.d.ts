export interface DiscordRole {
    id: string;
    name: string;
    color: number;
    hoist: boolean;
    position: number;
    permissions: string;
    managed: boolean;
    mentionable: boolean;
    tags?: {
        bot_id?: string;
        integration_id?: string;
        premium_subscriber?: null;
    };
}