import { Head, usePage } from "@inertiajs/react";
import HomeLayout from "@/Layouts/HomeLayout.js";
import { PageProps } from "@/Types/PageProps";
import DashboardLayout from "@/Layouts/DashboardLayout";
import {
    IconChartLine,
    IconClock,
    IconHammer,
    IconTicket,
} from "@tabler/icons-react";
import ModuleCard from "@/Components/ModuleCard";
import FakeDiscordMessage from "@/Components/FakeDiscordMessage";
import { useState } from "react";
import ConfirmPopup from "@/Components/ConfirmPopup";

// modules with name, description, link and icon (component)
// Your ban appeals, Your tickets, Your reminders, Your server levels
const modules = [
    {
        name: "Ban Appeals",
        description: "View statuses of your ban appeals.",
        link: "/dashboard/user/appeals",
        icon: <IconHammer size={48} color="red" />,
    },
    {
        name: "Tickets",
        description: "View statuses of your support tickets.",
        link: "/dashboard/user/tickets",
        icon: <IconTicket size={48} color="orange" />,
    },
    {
        name: "Reminders",
        description: "Display your set reminders.",
        link: "/dashboard/user/reminders",
        icon: <IconClock size={48} color="#0023FF" />,
    },
    {
        name: "Server Levels",
        description: "Display your server levels.",
        link: "/dashboard/user/levels",
        icon: <IconChartLine size={48} color="cyan" />,
    },
];

export default function Index({ user, application }: PageProps) {
    const authenticated = user != null;

    const [modalOpen, setModalOpen] = useState(true);

    if (!authenticated) window.location.href = "/login";

    return (
        <>
            <Head title="Welcome" />

            <DashboardLayout>
                <div className="md:flex items-center mb-4">
                    <img
                        src={user!.avatar}
                        className="inline-block h-16 w-16 rounded-full"
                        alt="User Avatar"
                    />
                    <h1 className="text-3xl font-extrabold tracking-tight text-white ml-4">
                        Welcome, {user!.username}!
                    </h1>
                </div>

                <h3 className="text-xl font-semibold my-4 text-white">
                    Your Modules
                </h3>
                <div className="w-full">
                    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mt-4">
                        {modules.map((module, index) => (
                            <ModuleCard 
                                key={index} 
                                name={module.name} 
                                description={module.description} 
                                link={module.link} 
                                icon={module.icon}
                            />
                        ))}
                    </div>
                </div>

                <FakeDiscordMessage
                    content="Welcome to the ModCore Dashboard! Here you can manage your ban appeals, tickets, reminders, and server levels."
                    className="mt-8"
                    embeds={[{
                        title: "Welcome to ModCore!",
                        description: "ModCore is your assistant for Discord server moderation and management through a wide range of hand-crafted features to make your life as a moderator or administrator a breeze!",
                        thumbnail: {
                            url: `https://cdn.discordapp.com/app-icons/${application.id}/${application.icon}.png`,
                        },
                        fields: [
                            {
                                name: "Dashboard",
                                value: "Manage your ban appeals, tickets, reminders, server levels, and more.",
                                inline: true,
                            },
                            {
                                name: "Support",
                                value: "Need help? Join our support server!",
                                inline: true,
                            },
                            {
                                name: "GitHub",
                                value: "Contribute to ModCore on GitHub! github.com/Naamloos/ModCore",
                                inline: false,
                            }
                        ],
                        footer: {
                            icon_url: user!.avatar,
                            text: "For " + user!.username + " <3",
                        }
                    }]}
                />
            </DashboardLayout>
        </>
    );
}
