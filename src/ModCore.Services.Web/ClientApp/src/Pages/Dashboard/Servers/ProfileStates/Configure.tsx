import DashboardLayout from "@/Layouts/DashboardLayout";
import { DiscordGuild } from "@/Types/DiscordTypes/DiscordGuild";
import { PagePropsWith } from "@/Types/PageProps";
import { Button } from "@/Components/ui/button";

import { Switch } from "@/Components/ui/switch";
import { useState } from "react";
import { router, useForm } from "@inertiajs/react";
import { ModCoreGuild } from "@/Types/DatabaseTypes/ModCoreGuild";

import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/Components/ui/card";

interface ConfigureProfileStatesProps {
  server: DiscordGuild;
  databaseServer: ModCoreGuild;
}

export default function Configure({
  server,
  databaseServer,
}: PagePropsWith<ConfigureProfileStatesProps>) {
  const [persistUserRoles, setPersistUserRoles] = useState(
    databaseServer.persist_user_roles,
  );
  const [persistUserOverrides, setPersistUserOverrides] = useState(
    databaseServer.persist_user_overrides,
  );
  const [persistUserNicknames, setPersistUserNicknames] = useState(
    databaseServer.persist_user_nicknames,
  );

  let icon = server?.icon ? server.icon : null;
  if (icon) {
    const ext = icon.includes("a_") ? ".gif" : ".png";
    icon = "https://cdn.discordapp.com/icons/" + server.id + "/" + icon + ext;
  } else {
    icon = "https://cdn.discordapp.com/embed/avatars/0.png";
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // Handle form submission here
    console.log({
      persistUserRoles,
      persistUserOverrides,
      persistUserNicknames,
    });
  };

  const { data } = useForm({
    persistUserRoles,
    persistUserOverrides,
    persistUserNicknames,
  });

  return (
    <>
      <DashboardLayout>
        <Button
          variant={"outline"}
          onClick={() => router.visit(`/dashboard/servers/${server.id}`)}
          className="mb-4"
        >
          Back to Overview
        </Button>
        <div className="md:flex items-center mb-4">
          <img
            src={icon}
            className="inline-block h-16 w-16 rounded-full"
            alt="Server Icon"
          />
          <h1 className="text-3xl font-extrabold tracking-tight text-white ml-4">
            {server.name}: Profile States Configuration
          </h1>
        </div>
        <Card className="w-full bg-gray-900 p-6 rounded-lg shadow-lg">
          <CardHeader>
            <CardTitle className="text-2xl">
              Profile States Configuration
            </CardTitle>
            <CardDescription>
              Configure how user profiles are persisted when users leave and
              rejoin the server. Enable or disable the persistence of roles,
              overrides, and nicknames.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <form className="space-y-8" onSubmit={handleSubmit}>
              <div className="flex flex-row items-center justify-between rounded-lg border p-4">
                <div className="space-y-0.5">
                  <label
                    htmlFor="persistUserRoles"
                    className="text-base text-white"
                  >
                    Persist User Roles
                  </label>
                  <p className="text-sm text-gray-500 dark:text-gray-400">
                    When enabled, user roles will be remembered and reassigned
                    if they leave and rejoin.
                  </p>
                </div>
                <Switch
                  id="persistUserRoles"
                  checked={persistUserRoles}
                  onCheckedChange={setPersistUserRoles}
                />
              </div>

              <div className="flex flex-row items-center justify-between rounded-lg border p-4">
                <div className="space-y-0.5">
                  <label
                    htmlFor="persistUserOverrides"
                    className="text-base text-white"
                  >
                    Persist User Overrides
                  </label>
                  <p className="text-sm text-gray-500 dark:text-gray-400">
                    When enabled, user permission overrides will be remembered
                    and reapplied if they leave and rejoin.
                  </p>
                </div>
                <Switch
                  id="persistUserOverrides"
                  checked={persistUserOverrides}
                  onCheckedChange={setPersistUserOverrides}
                />
              </div>

              <div className="flex flex-row items-center justify-between rounded-lg border p-4">
                <div className="space-y-0.5">
                  <label
                    htmlFor="persistUserNicknames"
                    className="text-base text-white"
                  >
                    Persist User Nicknames
                  </label>
                  <p className="text-sm text-gray-500 dark:text-gray-400">
                    When enabled, user nicknames will be remembered and
                    reapplied if they leave and rejoin.
                  </p>
                </div>
                <Switch
                  id="persistUserNicknames"
                  checked={persistUserNicknames}
                  onCheckedChange={setPersistUserNicknames}
                />
              </div>

              <Button type="submit" className="w-full sm:w-auto">
                Save Changes
              </Button>
            </form>
          </CardContent>
        </Card>
      </DashboardLayout>
    </>
  );
}
