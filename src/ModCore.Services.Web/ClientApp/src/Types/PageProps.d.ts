import { DiscordApplication } from "./DiscordTypes/DiscordApplication";
import { User } from "./User";

export type PageProps = {
  user: User | null;
  application: DiscordApplication;
};

export type PagePropsWith<T> = PageProps & T;
