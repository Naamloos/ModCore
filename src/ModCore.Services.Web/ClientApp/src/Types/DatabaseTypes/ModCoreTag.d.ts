export default interface ModCoreTag {
    id: string;
    channel_id?: string;
    guild_id: string;
    name: string;
    author_id: string;
    content: string;
    created_at: string;
    modified_at: string;
}

export interface ModCoreTagHistory
{
    id: string;
    tag_id: string;
    content: string;
    timestamp: string;
}