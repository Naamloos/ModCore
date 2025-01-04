import DiscordRolePicker from "@/Components/Forms/DiscordRolePicker";
import ToggleSwitch from "@/Components/Forms/ToggleSwitch";
import DashboardLayout from "@/Layouts/DashboardLayout";
import { ModCoreGuild } from "@/Types/DatabaseTypes/ModCoreGuild";
import { DiscordGuild } from "@/Types/DiscordTypes/DiscordGuild";
import { DiscordRole } from "@/Types/DiscordTypes/DiscordRole";
import { PagePropsWith } from "@/Types/PageProps";
import { useState } from "react";

interface ConfigureAutoRoleProps {
    server: DiscordGuild;
    databaseGuild: ModCoreGuild;
    enabled: boolean;
    roles: DiscordRole[];
}

interface UpdateAutorolesRequest {
    enabled: boolean;
    removeRoles: string[];
    addRoles: string[];
}

export default function Configure({
    user,
    server,
    databaseGuild,
    enabled,
    roles,
}: PagePropsWith<ConfigureAutoRoleProps>) {
    const [enabledRoles, setEnabledRoles] = useState<string[]>([]);
    return (
        <>
            <DashboardLayout>
                {/* List added roles, based on role names in roles prop. Also add a box to add a new role */}
                <h1 className="text-2xl font-bold text-white">
                    Auto Role Configuration
                </h1>
                <p className="text-white mt-2">
                    Configure the roles that are automatically assigned to new
                    members when they join the server.
                </p>
                <form
                    action={`/dashboard/servers/${server.id}/auto-role`}
                    method="POST"
                    className="mt-4 md:w-1/2"
                >
                    <input type="hidden" name="_method" value="PUT" />
                    <input
                        type="hidden"
                        name="enabled"
                        value={enabled ? "true" : "false"}
                    />
                    <div className="flex items-center justify-start">
                        <ToggleSwitch
                            label="Enable Auto Role"
                            enabled={enabled}
                            onUpdate={(enabled) => {}}
                        />
                    </div>
                    <div className="mt-2">
                        <div className="mb-5">
                            <DiscordRolePicker
                                label="Add Role"
                                placeholder="Select a role..."
                                value=""
                                onUpdate={(role) =>
                                    setEnabledRoles([...enabledRoles, role])
                                }
                                required
                            />
                        </div>
                        <label className="block mb-2 text-sm font-medium text-white">
                            Automatically Granted Roles
                        </label>
                        <div className="flex flex-col mt-2">
                            {enabledRoles.map((role) => {
                                // find role name from roles prop
                                let name =
                                    roles.find((r) => r.id === role)?.name ??
                                    "Unknown Role";
                                return (
                                    <>
                                        <div
                                            key={role}
                                            className="bg-gray-800 p-2 rounded-lg mb-2"
                                        >
                                            <span className="text-white">
                                                {name} ({role})
                                            </span>
                                            <button
                                                type="button"
                                                className="ml-2 text-red-500 float-right inline-block"
                                                onClick={() =>
                                                    setEnabledRoles(
                                                        enabledRoles.filter(
                                                            (r) => r !== role
                                                        )
                                                    )
                                                }
                                            >
                                                ✕
                                            </button>
                                        </div>
                                    </>
                                );
                            })}
                        </div>
                    </div>
                </form>
            </DashboardLayout>
        </>
    );
}
