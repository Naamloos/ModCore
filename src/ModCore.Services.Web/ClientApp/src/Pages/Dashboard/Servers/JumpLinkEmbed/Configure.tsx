import DashboardLayout from "@/Layouts/DashboardLayout";
import { DiscordGuild } from "@/Types/DiscordTypes/DiscordGuild";
import { PagePropsWith } from "@/Types/PageProps";
import { Button } from "@/Components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/Components/ui/card";
import { Switch } from "@/Components/ui/switch";
import { useState } from "react";
import { router, useForm } from "@inertiajs/react";
import { ModCoreGuild } from "../../../../Types/DatabaseTypes/ModCoreGuild";

interface ConfigureLevelingProps {
  server: DiscordGuild;
  databaseServer: ModCoreGuild;
}

export default function Configure({
  server,
  databaseServer,
}: PagePropsWith<ConfigureLevelingProps>) {
  const [disabled, setDisabled] = useState(
    databaseServer.embed_message_links_state === 0,
  );
  const [prefixed, setPrefixed] = useState(
    databaseServer.embed_message_links_state === 1,
  );
  const [always, setAlways] = useState(
    databaseServer.embed_message_links_state === 2,
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

    let state = 0;
    if (always) state = 2;
    else if (prefixed) state = 1;

    // Handle form submission here
    console.log({ state });
  };

  const { data } = useForm({
    state: databaseServer.embed_message_links_state,
  });

  // Ensure only one option is selected at a time
  const handleDisabledChange = (checked: boolean) => {
    if (checked) {
      setDisabled(true);
      setPrefixed(false);
      setAlways(false);
    } else if (!prefixed && !always) {
      setDisabled(true);
    }
  };

  const handlePrefixedChange = (checked: boolean) => {
    if (checked) {
      setDisabled(false);
      setPrefixed(true);
      setAlways(false);
    } else if (!disabled && !always) {
      setDisabled(true);
    }
  };

  const handleAlwaysChange = (checked: boolean) => {
    if (checked) {
      setDisabled(false);
      setPrefixed(false);
      setAlways(true);
    } else if (!disabled && !prefixed) {
      setDisabled(true);
    }
  };

  return (
    <>
      <DashboardLayout>
        <div className="md:flex items-center mb-4">
          <img
            src={icon}
            className="inline-block h-16 w-16 rounded-full mr-4"
            alt="User Avatar"
          />
          <h1 className="text-3xl font-extrabold tracking-tight text-white py-2">
            {server.name}: Jump Link Preview Configuration
          </h1>
          <Button
            variant="outline"
            onClick={() => router.visit(`/dashboard/servers/${server.id}`)}
            className="ml-auto self-center"
          >
            Back to Overview
          </Button>
        </div>
        <Card className="w-full bg-gray-900 p-6 rounded-lg shadow-lg">
          <CardHeader>
            <CardTitle className="text-2xl">
              Jump Link Preview Configuration
            </CardTitle>
            <CardDescription>
              Configure the jump link preview settings for this server.
              <br />
              This setting controls if, and how jump links are embedded in
              messages.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <form className="space-y-8" onSubmit={handleSubmit}>
              <div className="flex flex-row items-center justify-between rounded-lg border p-4">
                <div className="space-y-0.5">
                  <label htmlFor="disabled" className="text-base text-white">
                    Disabled
                  </label>
                  <p className="text-sm text-gray-500 dark:text-gray-400">
                    No jump link previews will be shown.
                  </p>
                </div>
                <Switch
                  id="disabled"
                  checked={disabled}
                  onCheckedChange={handleDisabledChange}
                />
              </div>

              <div className="flex flex-row items-center justify-between rounded-lg border p-4">
                <div className="space-y-0.5">
                  <label htmlFor="prefixed" className="text-base text-white">
                    Prefixed
                  </label>
                  <p className="text-sm text-gray-500 dark:text-gray-400">
                    Jump link previews will be shown, but only if the link
                    starts with an exclamation mark.
                  </p>
                </div>
                <Switch
                  id="prefixed"
                  checked={prefixed}
                  onCheckedChange={handlePrefixedChange}
                />
              </div>

              <div className="flex flex-row items-center justify-between rounded-lg border p-4">
                <div className="space-y-0.5">
                  <label htmlFor="always" className="text-base text-white">
                    Always
                  </label>
                  <p className="text-sm text-gray-500 dark:text-gray-400">
                    Jump link previews will be shown, regardless of the message
                    content.
                  </p>
                </div>
                <Switch
                  id="always"
                  checked={always}
                  onCheckedChange={handleAlwaysChange}
                />
              </div>

              <Button type="submit">Submit</Button>
            </form>
          </CardContent>
        </Card>
      </DashboardLayout>
    </>
  );
}
