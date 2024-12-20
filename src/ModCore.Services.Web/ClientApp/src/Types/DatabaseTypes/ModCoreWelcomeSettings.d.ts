import { DiscordEmbed } from "../DiscordTypes/DiscordEmbed";

export type ModCoreWelcomeSettings = {
    channel_id: bigint;
    welcome_message_json: string;
    enabled: boolean;
};

export type WelcomeMessageJson = {
    content: string;
    embeds: DiscordEmbed[];
}