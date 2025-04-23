export default interface ModCoreTimer {
  timer_id: string;
  guild_id: string;
  shard_id: string;
  trigger_at: string;
  type: TimerTypes;
  data: string;
}

export enum TimerTypes {
  Reminder = 0,
  Unban = 1,
  GuildDataDeletion = 2,
}

export interface ReminderTimerData {
  channel_id: string;
  user_id: string;
  text: string;
  snoozed: boolean;
  created_at: string;
}

export interface UnbanTimerData {
  user_id: string;
  display_name: string;
}
