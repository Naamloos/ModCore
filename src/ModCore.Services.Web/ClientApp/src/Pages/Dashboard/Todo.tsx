import { Head, router, usePage } from "@inertiajs/react";
import HomeLayout from "@/Layouts/HomeLayout.js";
import { PageProps, PagePropsWith } from "@/Types/PageProps";
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
import ConfirmPopup from "@/Components/ConfirmPopup";
import { useState } from "react";

export default function Todo({ user }: PageProps) {

    return (
        <>
            <Head title="Welcome" />

            <DashboardLayout>
                <h1 className="text-center text-3xl font-extrabold tracking-tight text-white">
                    Coming soon!
                </h1>
                <p className="text-center text-white mt-4">
                    This module has not yet been implemented! Please check back
                    later.
                </p>
                <p className="text-center text-white mt-4">
                    <a
                        onClick={() => window.history.back()}
                        className="text-blue-400 hover:text-blue-300 cursor-pointer"
                    >
                        Return to previous page
                    </a>
                </p>
            </DashboardLayout>
        </>
    );
}
