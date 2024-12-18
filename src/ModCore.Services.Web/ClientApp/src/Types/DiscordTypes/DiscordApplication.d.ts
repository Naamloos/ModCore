export type DiscordApplication = 
{
    id: string,
    icon: string,
    name: string,
    description: string,
    rpc_origins?: string[],
    bot_public: boolean,
    bot_require_code_grant: boolean,
    terms_of_service_url?: string,
    privacy_policy_url?: string,
    owner: {
        id: string,
        username: string,
        discriminator: string,
        avatar?: string
    },
    verify_key: string,
    guild_id?: string,
    primary_sku_id?: string,
    slug?: string,
    cover_image?: string,
    flags?: number
}
