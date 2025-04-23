import { ModCoreStarboardItem } from "./ModCoreStarboardItem";

export default interface ModCoreStarboard {
  id?: string;
  guild_id: string;
  enabled: boolean;
  minimum_reactions: number;
  emoji: string;
  channel_id: string;
  items?: ModCoreStarboardItem[];
}
