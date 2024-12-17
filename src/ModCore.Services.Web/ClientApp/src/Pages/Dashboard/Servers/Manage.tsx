import { Head, usePage } from "@inertiajs/react";
import MainLayout from "@/Layouts/MainLayout.js";
import { PagePropsWith } from "@/Types/PageProps";

export default function Index({user, server, permissions, databaseServer} : PagePropsWith<{server: any, permissions: any, databaseServer: any}>) 
{
    const authenticated = user != null;

    if(!authenticated)
        window.location.href = "/login";

    console.log(databaseServer);

    let icon = server?.icon ? server.icon : null;
    if (icon) {
        let ext = icon.includes("a_") ? ".gif" : ".png";
        icon = "https://cdn.discordapp.com/icons/" + server.id + "/" + icon + ext;
    } else {
        icon = "https://cdn.discordapp.com/embed/avatars/0.png";
    }

    function snowflakeToDate(snowflake : string) {
        const epoch = 1420070400000; // Discord epoch in milliseconds
        const timestamp = BigInt(snowflake) >> 22n;
        return new Date(Number(timestamp) + epoch);
    }

    const serverCreationDate = server? snowflakeToDate(server.id).toLocaleDateString() : null;

    return (
        <>
            <Head title="Welcome" />

            <MainLayout>
                <div className="bg-gray-900 p-6 rounded-lg shadow-lg">
                    {(server == null) && <>
                        <h1 className="text-center text-3xl font-extrabold tracking-tight text-white">
                            ModCore is not in this server!
                        </h1>
                        <p className="text-center text-white mt-4">
                            Please contact the server owner to add ModCore.
                        </p>
                        <p className="text-center text-white mt-4">
                            <a href="/dashboard/servers" className="text-blue-400 hover:text-blue-300">Back to Servers</a>
                        </p>
                    </>}
                    {server && <>
                        {/* Discord server list */}
                        <h1 className="text-center text-3xl font-extrabold tracking-tight text-white break-words">
                            Manage Server
                        </h1>

                        <div className="mt-4 text-white">
                            <div className="flex items-center">
                                <img src={icon} alt="Server Icon" className="w-16 h-16 rounded-full mr-4" />
                                <div>
                                    <h2 className="text-2xl font-bold break-words">{server.name}</h2>
                                    <p className="text-sm">{server.description || "No description available"}</p>
                                </div>
                            </div>
                            <div className="mt-4">
                                <h3 className="text-xl font-semibold">Server Details</h3>
                                <ul className="list-disc list-inside">
                                    <li><strong>Server ID:</strong> {server.id}</li>
                                    <li><strong>Owner ID:</strong> {server.owner_id}</li>
                                    <li><strong>Member Count:</strong> ~{server.approximate_member_count.value}</li>
                                    <li><strong>Created At:</strong> {serverCreationDate}</li>
                                </ul>

                                {/* Create a menu for managing the following options: AutoRole, BanAppeal, Infractions, Levels, Logging, ProfileStates, Rolemenus, Starboards, Tags, Tickets, Welcomer */}
                                <h3 className="text-xl font-semibold my-2">Manage Modules</h3>
                                <div className="grid grid-cols-2 lg:grid-cols-3 gap-2">
                                    <a href={"/dashboard/servers/" + server.id + "/autorole"} className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                                        🤖 AutoRole
                                    </a>
                                    <a href={"/dashboard/servers/" + server.id + "/banappeal"} className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                                        🔨 Ban Appeal
                                    </a>
                                    <a href={"/dashboard/servers/" + server.id + "/infractions"} className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                                        🤬 Infractions
                                    </a>
                                    <a href={"/dashboard/servers/" + server.id + "/levels"} className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                                        📈 Levels
                                    </a>
                                    <a href={"/dashboard/servers/" + server.id + "/logging"} className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                                        🪵 Logging
                                    </a>
                                    <a href={"/dashboard/servers/" + server.id + "/profilestates"} className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                                        🗿 Profile States
                                    </a>
                                    <a href={"/dashboard/servers/" + server.id + "/rolemenus"} className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                                        📖 Role Menus
                                    </a>
                                    <a href={"/dashboard/servers/" + server.id + "/starboards"} className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                                        ⭐ Starboards
                                    </a>
                                    <a href={"/dashboard/servers/" + server.id + "/tags"} className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                                        🏷️ Tags
                                    </a>
                                    <a href={"/dashboard/servers/" + server.id + "/tickets"} className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                                        🎫 Tickets
                                    </a>
                                    <a href={"/dashboard/servers/" + server.id + "/welcomer"} className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                                        👋 Welcomer
                                    </a>
                                    <a href={"/dashboard/servers/" + server.id + "/welcomer"} className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                                        📝 Nickname Approval
                                    </a>
                                    <a href={"/dashboard/servers/" + server.id + "/welcomer"} className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                                        🖇️ Jump Link Embed
                                    </a>
                                    <a href={"/dashboard/servers/" + server.id + "/datadump"} className="bg-gray-600 hover:bg-gray-700 text-white font-bold py-2 px-4 rounded">
                                        📩 Dump JSON (Advanced)
                                    </a>
                                </div>

                                {/* List permissions (list of strings) */}
                                <h3 className="text-xl font-semibold mt-2">Your permissions in this Guild</h3>
                                <div className="whitespace-pre-wrap text-xs">
                                    {permissions.map((feature : any) => {
                                        return (
                                            <span className="inline-block bg-gray-800 text-orange-400 px-2 py-1 my-1 rounded-md mr-2">
                                                {feature}
                                            </span>
                                        );
                                    })}
                                </div>

                                {/* List server.features (list of strings) */}
                                <h3 className="text-xl font-semibold mt-2">Discord Guild Features</h3>
                                <div className="whitespace-pre-wrap text-xs">
                                    {server.features.map((feature : any) => {
                                        return (
                                            <span className="inline-block bg-gray-800 text-orange-400 px-2 py-1 my-1 rounded-md mr-2">
                                                {feature}
                                            </span>
                                        );
                                    })}
                                </div>
                                {/* Danger Zone */}
                                <h3 className="text-xl text-red-300 font-semibold my-2">⚠️ Danger Zone ⚠️</h3>
                                <div className="grid grid-cols-2 gap-2">
                                    <a href={"/dashboard/servers/" + server.id + "/reset"} className="bg-red-500 hover:bg-red-700 text-white font-bold py-2 px-4 rounded">
                                        Reset Server
                                    </a>
                                    <a href={"/dashboard/servers/" + server.id + "/leave"} className="bg-red-500 hover:bg-red-700 text-white font-bold py-2 px-4 rounded">
                                        Leave Server
                                    </a>
                                </div>
                            </div>
                        </div>
                    </>}
                </div>
            </MainLayout>
        </>
    );
}
