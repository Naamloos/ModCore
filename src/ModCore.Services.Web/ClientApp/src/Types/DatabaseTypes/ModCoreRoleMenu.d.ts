import { ModCoreRoleMenuRole } from "./ModCoreRoleMenuRole";

export type ModCoreRoleMenu =
{
    id: bigint;
    guild_id: bigint;
    name: string;
    creator_id: bigint;
    roles: ModCoreRoleMenuRole[];
}