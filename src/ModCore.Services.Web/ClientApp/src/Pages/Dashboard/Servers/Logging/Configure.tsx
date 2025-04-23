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
import ModCoreLoggerSettings from "../../../../Types/DatabaseTypes/ModCoreLoggerSettings";
import { router, useForm } from "@inertiajs/react";
import { Label } from "@/Components/ui/label";
import DiscordChannelPicker from "../../../../Components/Forms/DiscordChannelPicker";

interface ConfigureLoggingProps {
  server: DiscordGuild;
  settings: ModCoreLoggerSettings;
}

export default function Configure({
  server,
  settings,
}: PagePropsWith<ConfigureLoggingProps>) {
  let icon = server?.icon ? server.icon : null;
  if (icon) {
    const ext = icon.includes("a_") ? ".gif" : ".png";
    icon = "https://cdn.discordapp.com/icons/" + server.id + "/" + icon + ext;
  } else {
    icon = "https://cdn.discordapp.com/embed/avatars/0.png";
  }

  const { data, setData } = useForm({
    loggerChannelId: settings.logger_channel_id || "",
    logJoins: settings.log_joins || false,
    logMessageEdit: settings.log_message_edit || false,
    logNicknames: settings.log_nicknames || false,
    logAvatars: settings.log_avatars || false,
    logInvites: settings.log_invites || false,
    logChannels: settings.log_channels || false,
    logGuildEdit: settings.log_guild_edit || false,
    logRoleEdit: settings.log_role_edit || false,
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // Handle form submission here
    console.log(data);
  };

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
            alt="User Avatar"
          />
          <h1 className="text-3xl font-extrabold tracking-tight text-white ml-4">
            {server.name}: Logging Configuration
          </h1>
        </div>
        <Card className="w-full bg-gray-900 p-6 rounded-lg shadow-lg">
          <CardHeader>
            <CardTitle className="text-2xl">Logging Configuration</CardTitle>
            <CardDescription>
              Configure the logging settings for this server.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit}>
              <div className="grid gap-4">
                <div className="space-y-2">
                  <DiscordChannelPicker
                    label="Select Logger Channel"
                    placeholder="Select a channel"
                    value={data.loggerChannelId}
                    onUpdate={(value) =>
                      setData("loggerChannelId", value || "")
                    }
                    required
                  />
                  <p className="text-sm text-muted-foreground">
                    The channel where log messages will be sent.
                  </p>
                </div>

                <div className="flex items-center justify-between rounded-lg border p-4">
                  <div className="space-y-0.5">
                    <Label htmlFor="logJoins" className="text-base">
                      Log Joins
                    </Label>
                    <p className="text-sm text-muted-foreground">
                      Log when users join the server.
                    </p>
                  </div>
                  <Switch
                    id="logJoins"
                    checked={data.logJoins}
                    onCheckedChange={(checked) => setData("logJoins", checked)}
                  />
                </div>

                <div className="flex items-center justify-between rounded-lg border p-4">
                  <div className="space-y-0.5">
                    <Label htmlFor="logMessageEdit" className="text-base">
                      Log Message Edits
                    </Label>
                    <p className="text-sm text-muted-foreground">
                      Log when messages are edited.
                    </p>
                  </div>
                  <Switch
                    id="logMessageEdit"
                    checked={data.logMessageEdit}
                    onCheckedChange={(checked) =>
                      setData("logMessageEdit", checked)
                    }
                  />
                </div>

                <div className="flex items-center justify-between rounded-lg border p-4">
                  <div className="space-y-0.5">
                    <Label htmlFor="logNicknames" className="text-base">
                      Log Nicknames
                    </Label>
                    <p className="text-sm text-muted-foreground">
                      Log when users change their nicknames.
                    </p>
                  </div>
                  <Switch
                    id="logNicknames"
                    checked={data.logNicknames}
                    onCheckedChange={(checked) =>
                      setData("logNicknames", checked)
                    }
                  />
                </div>

                <div className="flex items-center justify-between rounded-lg border p-4">
                  <div className="space-y-0.5">
                    <Label htmlFor="logAvatars" className="text-base">
                      Log Avatars
                    </Label>
                    <p className="text-sm text-muted-foreground">
                      Log when users change their avatars.
                    </p>
                  </div>
                  <Switch
                    id="logAvatars"
                    checked={data.logAvatars}
                    onCheckedChange={(checked) =>
                      setData("logAvatars", checked)
                    }
                  />
                </div>

                <div className="flex items-center justify-between rounded-lg border p-4">
                  <div className="space-y-0.5">
                    <Label htmlFor="logInvites" className="text-base">
                      Log Invites
                    </Label>
                    <p className="text-sm text-muted-foreground">
                      Log when invites are created or deleted.
                    </p>
                  </div>
                  <Switch
                    id="logInvites"
                    checked={data.logInvites}
                    onCheckedChange={(checked) =>
                      setData("logInvites", checked)
                    }
                  />
                </div>

                <div className="flex items-center justify-between rounded-lg border p-4">
                  <div className="space-y-0.5">
                    <Label htmlFor="logChannels" className="text-base">
                      Log Channels
                    </Label>
                    <p className="text-sm text-muted-foreground">
                      Log when channels are created, updated, or deleted.
                    </p>
                  </div>
                  <Switch
                    id="logChannels"
                    checked={data.logChannels}
                    onCheckedChange={(checked) =>
                      setData("logChannels", checked)
                    }
                  />
                </div>

                <div className="flex items-center justify-between rounded-lg border p-4">
                  <div className="space-y-0.5">
                    <Label htmlFor="logGuildEdit" className="text-base">
                      Log Guild Edits
                    </Label>
                    <p className="text-sm text-muted-foreground">
                      Log when the guild is edited.
                    </p>
                  </div>
                  <Switch
                    id="logGuildEdit"
                    checked={data.logGuildEdit}
                    onCheckedChange={(checked) =>
                      setData("logGuildEdit", checked)
                    }
                  />
                </div>

                <div className="flex items-center justify-between rounded-lg border p-4">
                  <div className="space-y-0.5">
                    <Label htmlFor="logRoleEdit" className="text-base">
                      Log Role Edits
                    </Label>
                    <p className="text-sm text-muted-foreground">
                      Log when roles are edited.
                    </p>
                  </div>
                  <Switch
                    id="logRoleEdit"
                    checked={data.logRoleEdit}
                    onCheckedChange={(checked) =>
                      setData("logRoleEdit", checked)
                    }
                  />
                </div>
              </div>
              <Button type="submit" className="mt-2">
                Submit
              </Button>
            </form>
          </CardContent>
        </Card>
      </DashboardLayout>
    </>
  );
}
