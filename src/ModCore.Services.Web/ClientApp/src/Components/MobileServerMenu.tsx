import Sidebar from "@/Components/Sidebar";
import { DiscordGuild } from "@/Types/DiscordTypes/DiscordGuild";
import { User } from "@/Types/User";
import { usePage } from "@inertiajs/react";
import { IconCross, IconX } from "@tabler/icons-react";

export default function MobileServerMenu({ isMenuOpen, setIsMenuOpen} : {isMenuOpen: boolean, setIsMenuOpen: (value: boolean) => void}) 
{
    const { user_guilds, server } = usePage<{
        user_guilds: DiscordGuild[];
        server: DiscordGuild | undefined;
    }>().props;

    return (
        <>
            {isMenuOpen && (
                <div className="bg-gray-900 fixed inset-0 z-50 flex flex-col items-center py-4 overflow-auto">
                    <button
                        className="text-gray-400 hover:text-white transition-colors mb-4"
                        onClick={() => setIsMenuOpen(false)}
                    >
                        <div className="flex justify-center items-center h-6 w-6">
                            <IconX size={24} />
                        </div>
                    </button>
                    {user_guilds.map((discordGuild) => (
                        <div
                            key={discordGuild.id}
                            className={`relative group flex items-center w-full cursor-pointer mb-3 px-12${
                                discordGuild.id === (server?.id ?? 0) ? " bg-gray-700" : ""
                            }`}
                            onClick={() => {
                                window.location.href = `/dashboard/servers/${discordGuild.id}`;
                            }}
                        >
                            <button
                                className={`w-12 h-12 rounded-full transition-all duration-200 relative group flex-shrink-0 ${
                                    discordGuild.id === (server?.id ?? 0)
                                        ? "rounded-2xl"
                                        : "hover:rounded-2xl hover:bg-gray-700"
                                }`}
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
                            <span className="ml-4 text-white text-ellipsis whitespace-nowrap overflow-hidden">
                                {discordGuild.name}
                            </span>
                        </div>
                    ))}
                </div>
            )}
        </>
    );
}
