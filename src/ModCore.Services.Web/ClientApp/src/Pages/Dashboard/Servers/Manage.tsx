import { Head, usePage } from "@inertiajs/react";
import HomeLayout from "@/Layouts/HomeLayout.js";
import { PagePropsWith } from "@/Types/PageProps";
import { DiscordGuild } from "@/Types/DiscordTypes/DiscordGuild";
import { ModCoreGuild } from "@/Types/DatabaseTypes/ModCoreGuild";
import DashboardLayout from "@/Layouts/DashboardLayout";
import {
    IconAlertSquareRounded,
    IconBook,
    IconChartLine,
    IconHammer,
    IconHandMove,
    IconHandStop,
    IconJumpRope,
    IconMoodAngry,
    IconMoon,
    IconNotebook,
    IconRobot,
    IconStar,
    IconTag,
    IconTicket,
    IconTree,
} from "@tabler/icons-react";
import { hasPermissionsFromString } from "@/Types/DiscordTypes/DiscordPermission";
import ModuleCard from "@/Components/ModuleCard";

type ManagePageProps = {
    server: DiscordGuild;
    permissions: string[];
    databaseServer: ModCoreGuild;
};

// modules with name, description, link and icon (emoji)
const modules = (id : string) => [
    {
        name: "AutoRole",
        description: "Automatically assign roles to new members.",
        link: `/dashboard/servers/${id}/autoroles`,
        icon: <IconRobot size={48} color="#00BFFF" />, // DeepSkyBlue
    },
    {
        name: "Ban Appeal",
        description: "Allow members to appeal their bans.",
        link: `/dashboard/servers/${id}/appeals`,
        icon: <IconHammer size={48} color="#8B4513" />, // SaddleBrown
    },
    {
        name: "Infractions",
        description: "Track and manage member infractions.",
        link: `/dashboard/servers/${id}/infractions`,
        icon: <IconMoodAngry size={48} color="#FF4500" />, // OrangeRed
    },
    {
        name: "Levels",
        description: "Implement a leveling system for members.",
        link: `/dashboard/servers/${id}/levels`,
        icon: <IconChartLine size={48} color="#00CED1" />, // DarkTurquoise
    },
    {
        name: "Logging",
        description: "Log server events and activities.",
        link: `/dashboard/servers/${id}/logging`,
        icon: <IconTree size={48} color="#32CD32" />, // LimeGreen
    },
    {
        name: "Profile States",
        description: "Manage member profile states.",
        link: `/dashboard/servers/${id}/states`,
        icon: <IconMoon size={48} color="#4682B4" />, // SteelBlue
    },
    {
        name: "Role Menus",
        description: "Create and manage role menus.",
        link: `/dashboard/servers/${id}/rolemenus`,
        icon: <IconBook size={48} color="#1E90FF" />, // DodgerBlue
    },
    {
        name: "Starboards",
        description: "Set up starboards for starred messages.",
        link: `/dashboard/servers/${id}/starboards`,
        icon: <IconStar size={48} color="#FFD700" />, // Gold
    },
    {
        name: "Tags",
        description: "Create and manage custom tags.",
        link: `/dashboard/servers/${id}/tags`,
        icon: <IconTag size={48} color="#FFA500" />, // Orange
    },
    {
        name: "Tickets",
        description: "Manage support tickets.",
        link: `/dashboard/servers/${id}/tickets`,
        icon: <IconTicket size={48} color="#9370DB" />, // MediumPurple
    },
    {
        name: "Welcome Messages",
        description: "Set up welcome messages for new members.",
        link: `/dashboard/servers/${id}/welcome`,
        icon: <IconHandStop size={48} color="#3CB371" />, // MediumSeaGreen
    },
    {
        name: "Nickname Approval",
        description: "Approve or reject member nicknames.",
        link: `/dashboard/servers/${id}/nicknames`,
        icon: <IconNotebook size={48} color="#4169E1" />, // RoyalBlue
    },
    {
        name: "Jump Link Embed",
        description: "Embed jump links in messages.",
        link: `/dashboard/servers/${id}/jumplinkembed`,
        icon: <IconJumpRope size={48} color="#00FA9A" />, // MediumSpringGreen
    },
];

export default function Manage({
    user,
    server,
    databaseServer,
}: PagePropsWith<ManagePageProps>) {
    const authenticated = user != null;

    if (!authenticated) window.location.href = "/login";

    if (!hasPermissionsFromString(server.permissions, "MANAGE_GUILD")) {
        window.location.href = "/login";
    }

    console.log(databaseServer);
    console.log(server);

    let icon = server?.icon ? server.icon : null;
    if (icon) {
        let ext = icon.includes("a_") ? ".gif" : ".png";
        icon =
            "https://cdn.discordapp.com/icons/" + server.id + "/" + icon + ext;
    } else {
        icon = "https://cdn.discordapp.com/embed/avatars/0.png";
    }

    function snowflakeToDate(snowflake: string) {
        const epoch = 1420070400000; // Discord epoch in milliseconds
        const timestamp = BigInt(snowflake) >> 22n;
        return new Date(Number(timestamp) + epoch);
    }

    const serverCreationDate = server
        ? snowflakeToDate(server.id).toLocaleDateString()
        : null;

    return (
        <>
            <Head title="Welcome" />

            <DashboardLayout>
                {server == null && (
                    <>
                        <h1 className="text-center text-3xl font-extrabold tracking-tight text-white">
                            ModCore is not in this server!
                        </h1>
                        <p className="text-center text-white mt-4">
                            Please contact the server owner to add ModCore.
                        </p>
                        <p className="text-center text-white mt-4">
                            <a
                                href="/dashboard/servers"
                                className="text-blue-400 hover:text-blue-300"
                            >
                                Back to Servers
                            </a>
                        </p>
                    </>
                )}
                {server && (
                    <>
                        <div className="md:flex items-center mb-4">
                            <img
                                src={icon}
                                className="inline-block h-16 w-16 rounded-full"
                                alt="User Avatar"
                            />
                            <h1 className="text-3xl font-extrabold tracking-tight text-white ml-4">
                                {server.name}
                            </h1>
                        </div>

                        <div>
                            <p className="text-sm">
                                Members:{" "}
                                {server.approximate_member_count ??
                                    "Currently Unknown"}
                            </p>
                            <p className="text-sm">
                                Created on {serverCreationDate}
                            </p>
                            <p className="text-sm">
                                Server ID: {server.id}
                            </p>
                        </div>

                        <div className="mt-4">
                            {/* Create a menu for managing the following options: AutoRole, BanAppeal, Infractions, Levels, Logging, ProfileStates, Rolemenus, Starboards, Tags, Tickets, Welcomer */}
                            <h3 className="text-xl font-semibold my-4">
                                Manage Modules
                            </h3>
                            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mt-4">
                                {modules(server.id).map((module, index) => (
                                    <ModuleCard 
                                        key={index} 
                                        name={module.name} 
                                        description={module.description} 
                                        link={module.link} 
                                        icon={module.icon}
                                    />
                                ))}
                            </div>
                            {/* Danger Zone */}
                            <div className="border-red-500 border-solid border-2 rounded p-4 mt-16 sm:mx-16">
                                <h3 className="text-xl text-red-500 font-bold mb-4 text-center">
                                    <IconAlertSquareRounded className="inline" />
                                    &nbsp;Danger Zone&nbsp;
                                    <IconAlertSquareRounded className="inline" />
                                </h3>
                                <p className="text-center text-red-500 mb-4">
                                    These actions are irreversible! Proceed with
                                    caution!
                                </p>
                                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                                    <a
                                        href={
                                            "/dashboard/servers/" +
                                            server.id +
                                            "/reset"
                                        }
                                        className="bg-red-500 hover:bg-red-700 text-white font-bold py-2 px-4 rounded"
                                    >
                                        Reset Server
                                    </a>
                                    <a
                                        href={
                                            "/dashboard/servers/" +
                                            server.id +
                                            "/leave"
                                        }
                                        className="bg-red-500 hover:bg-red-700 text-white font-bold py-2 px-4 rounded"
                                    >
                                        Leave Server
                                    </a>
                                </div>
                            </div>
                        </div>
                    </>
                )}
            </DashboardLayout>
        </>
    );
}
