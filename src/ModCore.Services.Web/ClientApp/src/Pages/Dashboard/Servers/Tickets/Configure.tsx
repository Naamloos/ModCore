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

interface ConfigureTicketsProps {
    server: DiscordGuild;
    // Add any specific props for tickets configuration here
}

export default function Configure({
    user,
    server,
}: PagePropsWith<ConfigureTicketsProps>) {
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
                        {server.name}: Tickets Configuration
                    </h1>
                </div>
                <Card className="w-full bg-gray-900 p-6 rounded-lg shadow-lg">
                    <CardHeader>
                        <CardTitle className="text-2xl">
                            Tickets Configuration
                        </CardTitle>
                        <CardDescription>
                            Configure the tickets system for this server.
                        </CardDescription>
                    </CardHeader>
                    <CardContent>
                        {/* Add your tickets configuration form elements here */}
                        <p className="text-white">
                            Tickets configuration will go here.
                        </p>
                        <div className="mt-4">
                            <h2 className="text-xl font-bold text-white">Existing Tickets</h2>
                            <ul className="list-disc list-inside text-white">
                                {/* Replace with dynamic ticket data */}
                                <li>Ticket 1: Issue with server</li>
                                <li>Ticket 2: Bug report</li>
                                <li>Ticket 3: Feature request</li>
                            </ul>
                        </div>
                        <div className="mt-4">
                            <h2 className="text-xl font-bold text-white">Manage Tickets</h2>
                            <Button variant={"outline"} className="mt-2">
                                Create New Ticket
                            </Button>
                        </div>
                    </CardContent>
                </Card>
            </DashboardLayout>
        </>
    );
}
