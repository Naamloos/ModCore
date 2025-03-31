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

    const handleToggle = (newEnabled: boolean) => {
        setIsEnabled(newEnabled);
    };

    const handleAddRole = (role: string) => {
        if (!enabledRoles.includes(role)) {
            setEnabledRoles([...enabledRoles, role]);
        }
    };

    const handleRemoveRole = (role: string) => {
        setEnabledRoles(enabledRoles.filter((r) => r !== role));
    };

    let icon = server?.icon ? server.icon : null;
    if (icon) {
        let ext = icon.includes("a_") ? ".gif" : ".png";
        icon =
            "https://cdn.discordapp.com/icons/" + server.id + "/" + icon + ext;
    } else {
        icon = "https://cdn.discordapp.com/embed/avatars/0.png";
    }

    return (
        <>
            <DashboardLayout>
                <Button variant={"outline"} onClick={()=> window.location.href = `/dashboard/servers/${server.id}`} className="mb-4">
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
                        <CardTitle className="text-2xl">
                            Auto Role Configuration
                        </CardTitle>
                        <CardDescription>
                            Configure the roles that are automatically assigned
                            to new members when they join the server.
                        </CardDescription>
                    </CardHeader>
                    <CardContent>
                        <form
                            action={`/dashboard/servers/${server.id}/auto-role`}
                            method="POST"
                        >
                            <input type="hidden" name="_method" value="PUT" />
                            <input
                                type="hidden"
                                name="enabled"
                                value={isEnabled ? "true" : "false"}
                            />
                            <div className="flex items-center justify-between space-x-2">
                                <label htmlFor="enabled">Enable Auto Role</label>
                                <Switch
                                    id="enabled"
                                    checked={isEnabled}
                                    onCheckedChange={handleToggle}
                                />
                            </div>
                            <div className="mt-4 space-y-4">
                                <DiscordRolePicker
                                    label="Add Role"
                                    placeholder="Select a role..."
                                    value=""
                                    onUpdate={handleAddRole}
                                    required
                                />

                                <div>
                                    <label className="block mb-2 text-sm font-medium">
                                        Automatically Granted Roles
                                    </label>
                                    <div className="flex flex-col mt-2">
                                        {enabledRoles.map((role) => {
                                            let name =
                                                roles.find((r) => r.id === role)
                                                    ?.name ?? "Unknown Role";
                                            return (
                                                <div
                                                    key={role}
                                                    className="bg-gray-800 p-2 rounded-lg mb-2 flex items-center justify-between"
                                                >
                                                    <span className="text-white">
                                                        {name} ({role})
                                                    </span>
                                                    <Button
                                                        type="button"
                                                        variant="destructive"
                                                        size="icon"
                                                        onClick={() =>
                                                            handleRemoveRole(role)
                                                        }
                                                    >
                                                        ✕
                                                    </Button>
                                                </div>
                                            );
                                        })}
                                    </div>
                                </div>
                            </div>
                            <CardFooter className="justify-end">
                                <Button type="submit">Save Changes</Button>
                            </CardFooter>
                        </form>
                    </CardContent>
                </Card>
            </DashboardLayout>
        </>
    );
}
