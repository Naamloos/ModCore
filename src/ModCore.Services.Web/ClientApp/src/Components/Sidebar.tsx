import { DiscordGuild } from "@/Types/DiscordTypes/DiscordGuild";
import { User } from "@/Types/User";
import { usePage } from "@inertiajs/react";
import { IconPlus } from "@tabler/icons-react";
import { useState, useEffect } from "react";

export default function Sidebar() {
    const { user_guilds, user, server } = usePage<{
        user_guilds: DiscordGuild[];
        user: User;
        server: DiscordGuild | undefined;
    }>().props;

    const [isCollapsed, setIsCollapsed] = useState(true);

    return (
        <>
            <div
                className={`bg-gray-900 h-screen fixed left-0 top-0 flex flex-col items-center py-4 border-r border-gray-800 transition-all duration-300 ${
                    isCollapsed ? "w-[4.5rem]" : "w-64"
                }`}
                style={{ zIndex: 1000 }}
            >
                <div
                    className="flex-1 overflow-y-scroll overflow-x-visible w-full px-2 block"
                    style={{ scrollbarWidth: "none", msOverflowStyle: "none" }}
                    // hover event
                    onMouseEnter={() => setIsCollapsed(false)}
                    onMouseLeave={() => setIsCollapsed(true)}
                >
                    {user_guilds.map((discordGuild) => (
                        <div
                            key={discordGuild.id}
                            className="relative group flex items-center w-full cursor-pointer hover:bg-gray-600 p-1 rounded-md mb-1"
                            onClick={() => {
                                window.location.href = `/dashboard/servers/${discordGuild.id}`;
                            }}
                        >
                            {discordGuild.id === (server?.id ?? 0) && (
                                <div className="absolute left-0 top-0 transform translate-y-3 -translate-x-2 w-1 h-8 bg-gray-100 rounded-full" />
                            )}
                            <button
                                className={`w-12 h-12 rounded-full transition-all duration-200 relative group flex-shrink-0`}
                            >
                                {discordGuild.icon ? (
                                    <img
                                        src={`https://cdn.discordapp.com/icons/${discordGuild.id}/${discordGuild.icon}.png`}
                                        alt={discordGuild.name}
                                        className="w-full h-full rounded-inherit object-cover"
                                    />
                                ) : (
                                    <div className="w-full h-full flex items-center justify-center rounded-inherit text-white font-medium">
                                        {discordGuild.name
                                            .split(" ")
                                            .map((word) => word.charAt(0))
                                            .join("")}
                                    </div>
                                )}
                            </button>
                            {!isCollapsed && (
                                <span className="ml-4 text-white text-ellipsis whitespace-nowrap overflow-hidden transition-opacity duration-300 opacity-100">
                                    {discordGuild.name}
                                </span>
                            )}
                            {isCollapsed && (
                                <span className="ml-4 text-white text-ellipsis whitespace-nowrap overflow-hidden transition-opacity duration-300 opacity-0">
                                    {discordGuild.name}
                                </span>
                            )}
                        </div>
                    ))}
                </div>
            </div>
        </>
    );
}
