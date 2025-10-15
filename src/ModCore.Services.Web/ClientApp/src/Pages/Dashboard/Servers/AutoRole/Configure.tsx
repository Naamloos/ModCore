import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/Components/ui/card";
import { Switch } from "@/Components/ui/switch";
import DiscordRolePicker from "@/Components/Forms/DiscordRolePicker";
import DashboardLayout from "@/Layouts/DashboardLayout";
import { ModCoreGuild } from "@/Types/DatabaseTypes/ModCoreGuild";
import { DiscordGuild } from "@/Types/DiscordTypes/DiscordGuild";
import { DiscordRole } from "@/Types/DiscordTypes/DiscordRole";
import { PagePropsWith } from "@/Types/PageProps";
import { useState } from "react";
import { Button } from "@/Components/ui/button";
import { Label } from "@/Components/ui/label";
import { Separator } from "@/Components/ui/separator";
import { X } from "lucide-react";
import { router } from "@inertiajs/react";

interface ConfigureAutoRoleProps {
  server: DiscordGuild;
  databaseGuild: ModCoreGuild;
  enabled: boolean;
  roles: DiscordRole[];
}

export default function Configure({
  server,
  enabled,
  roles,
}: PagePropsWith<ConfigureAutoRoleProps>) {
  const [enabledRoles, setEnabledRoles] = useState<string[]>([]);
  const [isEnabled, setIsEnabled] = useState(enabled);
  const [currentRoleId, setCurrentRoleId] = useState<string>("");

  const handleToggle = (newEnabled: boolean) => {
    setIsEnabled(newEnabled);
  };

  const handleAddRole = () => {
    if (currentRoleId && !enabledRoles.includes(currentRoleId)) {
      setEnabledRoles([...enabledRoles, currentRoleId]);
      setCurrentRoleId("");
    }
  };

  const handleRemoveRole = (role: string) => {
    setEnabledRoles(enabledRoles.filter((r) => r !== role));
  };

  let icon = server?.icon ? server.icon : null;
  if (icon) {
    const ext = icon.includes("a_") ? ".gif" : ".png";
    icon = "https://cdn.discordapp.com/icons/" + server.id + "/" + icon + ext;
  } else {
    icon = "https://cdn.discordapp.com/embed/avatars/0.png";
  }

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
            {server.name}: Auto Roles
          </h1>
        </div>
        <Card className="w-full bg-gray-900 p-6 rounded-lg shadow-lg">
          <CardHeader>
            <CardTitle className="text-2xl">Auto Role Configuration</CardTitle>
            <CardDescription>
              Configure the roles that are automatically assigned to new members
              when they join the server.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <form
              action={`/dashboard/servers/${server.id}/auto-role`}
              method="POST"
              className="space-y-6"
            >
              <input type="hidden" name="_method" value="PUT" />
              <input
                type="hidden"
                name="enabled"
                value={isEnabled ? "true" : "false"}
              />

              <div className="flex items-center justify-between space-x-2 py-2">
                <Label htmlFor="enabled" className="font-medium">
                  Enable Auto Role
                </Label>
                <Switch
                  id="enabled"
                  checked={isEnabled}
                  onCheckedChange={handleToggle}
                />
              </div>

              <Separator className="my-4" />

              <div className="space-y-4">
                <div className="space-y-2">
                  <Label htmlFor="role-picker">Add Role</Label>
                  <div className="flex gap-2 justify-center items-center">
                    <div className="flex-1">
                      <DiscordRolePicker
                        label=""
                        placeholder="Select a role to add"
                        value={currentRoleId}
                        onUpdate={setCurrentRoleId}
                        required={false}
                      />
                    </div>
                    <Button
                      type="button"
                      onClick={handleAddRole}
                      disabled={!currentRoleId}
                      className="mt-2"
                    >
                      Add Role
                    </Button>
                  </div>
                </div>

                <div className="space-y-2 mt-4">
                  <Label className="font-medium">
                    Automatically Granted Roles
                  </Label>

                  {enabledRoles.length > 0 ? (
                    <div className="space-y-2 border rounded-md p-4">
                      {enabledRoles.map((role) => {
                        const roleObj = roles.find((r) => r.id === role);
                        return (
                          <div
                            key={role}
                            className="flex items-center justify-between p-3 bg-gray-800 rounded-md"
                          >
                            <span className="font-medium">
                              {roleObj?.name || "Unknown Role"}
                            </span>
                            <Button
                              type="button"
                              variant="ghost"
                              size="sm"
                              onClick={() => handleRemoveRole(role)}
                              className="h-8 w-8 p-0"
                            >
                              <X size={16} />
                            </Button>
                          </div>
                        );
                      })}
                    </div>
                  ) : (
                    <div className="text-center p-4 border border-dashed rounded-md">
                      <p className="text-sm text-gray-500">
                        No roles added yet. Add at least one role to enable
                        auto-role functionality.
                      </p>
                    </div>
                  )}
                </div>
              </div>

              <CardFooter className="px-0 pt-4">
                <div className="flex justify-end w-full">
                  <Button type="submit" className="ml-auto">
                    Save Changes
                  </Button>
                </div>
              </CardFooter>
            </form>
          </CardContent>
        </Card>
      </DashboardLayout>
    </>
  );
}
