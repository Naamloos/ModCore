import ModCoreRoleMenuRole from "./ModCoreRoleMenuRole";

export default interface ModCoreRoleMenu {
  id: number;
  guild_id: string;
  name: string;
  creator_id: string;
  roles: ModCoreRoleMenuRole[];
}
