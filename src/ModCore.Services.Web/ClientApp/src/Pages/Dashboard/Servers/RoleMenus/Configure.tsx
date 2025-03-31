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
import { useState } from "react";
import ModCoreRoleMenu from "../../../../Types/DatabaseTypes/ModCoreRoleMenu";

interface ConfigureRoleMenusProps {
    server: DiscordGuild;
    menus: ModCoreRoleMenu[];
}

export default function Configure({
    user,
    server,
    menus,
}: PagePropsWith<ConfigureRoleMenusProps>) {
    let icon = server?.icon ? server.icon : null;
    if (icon) {
        let ext = icon.includes("a_") ? ".gif" : ".png";
        icon =
            "https://cdn.discordapp.com/icons/" + server.id + "/" + icon + ext;
    } else {
        icon = "https://cdn.discordapp.com/embed/avatars/0.png";
    }

    const [roleMenus, setRoleMenus] = useState([
        { id: 1, name: "Role Menu 1" },
        { id: 2, name: "Role Menu 2" },
    ]);

    return (
        <>
            <DashboardLayout>
                <Button
                    variant={"outline"}
                    onClick={() =>
                        (window.location.href = `/dashboard/servers/${server.id}`)
                    }
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
                        {server.name}: Role Menu Configuration
                    </h1>
                </div>
                <Card className="w-full bg-gray-900 p-6 rounded-lg shadow-lg">
                    <CardHeader>
                        <CardTitle className="text-2xl">
                            Role Menu Configuration
                        </CardTitle>
                        <CardDescription>
                            Manage role menus for this server.
                        </CardDescription>
                    </CardHeader>
                    <CardContent>
                        <ul>
                            {roleMenus.map((menu) => (
                                <li key={menu.id} className="text-white">
                                    {menu.name}
                                </li>
                            ))}
                        </ul>
                        <Button>Create New Role Menu</Button>
                    </CardContent>
                </Card>
            </DashboardLayout>
        </>
    );
}
