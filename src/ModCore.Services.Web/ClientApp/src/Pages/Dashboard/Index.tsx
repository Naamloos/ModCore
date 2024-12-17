import { Head, usePage } from "@inertiajs/react";
import MainLayout from "@/Layouts/MainLayout.js";

export default function Index(props : any) 
{
    const { user, application } = props;
    const authenticated = user != null;

    if(!authenticated)
        window.location.href = "/login";

    return (
        <>
            <Head title="Welcome" />

            <MainLayout>
                <div className="bg-gray-900 p-6 m-4 rounded-lg shadow-lg flex flex-col items-center">
                    <div className="text-center">
                        <div className="flex justify-center items-center mb-4">
                            <img src={`https://cdn.discordapp.com/app-icons/${application.id}/${application.icon}.png`} className="h-16 w-16" alt="Application Icon" />
                            <h1 className="text-3xl font-extrabold tracking-tight text-white ml-4">
                                Welcome to ModCore Dashboard
                            </h1>
                        </div>
                    </div>

                    <div className="mt-4 md:mt-0 md:ml-4 p-4 flex items-center w-full md:w-auto">
                        <div className="mr-4">
                            <img src={user.avatar} alt="User Avatar" className="h-12 w-12 rounded-full" />
                        </div>
                        <div>
                            <h2 className="text-xl font-bold text-white">{user.username}</h2>
                        </div>
                    </div>

                    <div className="mt-6 grid grid-flow-col gap-2">
                        <a href="/dashboard/servers" className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                            Manage Servers
                        </a>
                        <a href="/dashboard/servers" className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                            Manage Profile
                        </a>
                    </div>
                </div>
            </MainLayout>
        </>
    );
}
