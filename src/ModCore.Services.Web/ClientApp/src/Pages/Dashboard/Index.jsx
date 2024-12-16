import { Head, usePage } from "@inertiajs/react";
import MainLayout from "@/Layouts/MainLayout.jsx";
import Logo from "@/Resources/logo.png";

export default function IndexPage(props) 
{
    const { authenticated, user, application } = props;

    if(!authenticated)
        window.location.href = "/login";

    return (
        <>
            <Head title="Welcome" />

            <MainLayout>
                <div className="bg-gray-900 p-6 m-4 rounded-lg shadow-lg">
                    {/* Discord server list */}
                    <h1 className="text-center text-3xl font-extrabold tracking-tight text-white mb-6">
                        Your Servers
                    </h1>
                    <div className="flex justify-center">
                        <div className="grid grid-cols-1 gap-4 mt-8 max-w-xl">
                            {servers.map((server) => {
                                let icon = server.icon ? server.icon : null;
                                if (icon) {
                                    let ext = icon.includes("a_") ? ".gif" : ".png";
                                    icon = "https://cdn.discordapp.com/icons/" + server.id + "/" + icon + ext;
                                } else {
                                    icon = "https://cdn.discordapp.com/embed/avatars/0.png";
                                }
                                return (
                                    <a 
                                        href={"/dashboard/servers/" + server.id}
                                        className="cursor-pointer"
                                    >
                                        <div key={server.id} className="flex items-center bg-gray-800 p-4 rounded-lg shadow-md hover:bg-gray-700 transition duration-300 ease-in-out">
                                            <img src={icon} className="h-12 w-12 rounded-full mr-4" />
                                            <div className="flex-grow text-lg text-white truncate">
                                                {server.name}
                                            </div>
                                        </div>
                                    </a>
                                );
                            })}
                        </div>
                    </div>
                </div>
            </MainLayout>
        </>
    );
}
