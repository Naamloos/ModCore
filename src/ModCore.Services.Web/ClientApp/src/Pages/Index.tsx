import { Head, usePage } from "@inertiajs/react";
import MainLayout from "@/Layouts/MainLayout.js";
import { IconBrandGithub } from "@tabler/icons-react";
import { PagePropsWith } from "@/Types/PageProps";

type IndexPageProps =
{
    dotnetVersion: string
}

export default function IndexPage({ dotnetVersion, application } : PagePropsWith<IndexPageProps>) 
{
    return (
        <>
            <MainLayout>
                <Head title="Welcome" />
                <div className="text-center text-lg max-w-2xl mx-auto mt-8 px-4 sm:px-6 lg:px-8">
                    <div className="flex flex-col items-center justify-center mt-12 sm:mt-24 sm:flex-row pb-4">
                        <img src={`https://cdn.discordapp.com/app-icons/${application.id}/${application.icon}.png`} alt="Logo" className="mb-4 sm:mb-0 sm:mr-4 h-24 w-24 sm:h-36 sm:w-36" />
                        <h1 className="text-center sm:text-left text-4xl sm:text-6xl font-extrabold tracking-tight">
                            Welcome to <br />
                            <span className="bg-gradient-to-t from-[#089fdf] to-blue-100 bg-clip-text text-transparent">
                                ModCore
                            </span>
                        </h1>
                    </div>

                    <div className="bg-gray-900 py-6 pt-2 m-4 max-w-xl rounded-lg shadow-lg">
                        <p className="text-left text-lg max-w-xl mx-auto mt-8 px-4 sm:px-6 lg:px-8">
                            ModCore is your assistant for Discord server moderation and management 
                            through a wide range of hand-crafted features to make your life as a moderator or administrator a breeze!
                        </p>
                        <p className="text-center text-lg max-w-xl mx-auto mt-8 px-4 sm:px-6 lg:px-8 flex justify-center">
                            Built with&nbsp;
                            <a href="https://dot.net" target="_blank" className="text-blue-400 hover:text-blue-200">
                                .NET {dotnetVersion}
                            </a>
                            &nbsp;&nbsp;GitHub:&nbsp;
                            <a href="https://github.com/Naamloos/ModCore/" target="_blank" className="text-blue-400 hover:text-blue-200 inline-block justify-center">
                                <IconBrandGithub size={24} />
                            </a>
                        </p>
                    </div>

                    <div className="mt-12 max-w-2xl">
                        <div className="flex flex-col sm:flex-row items-center mb-8 bg-gray-900 py-6 px-2 m-4 max-w-xl rounded-lg shadow-lg">
                            <img src="https://placehold.co/600x400" alt="Feature 1" className="mb-4 sm:mb-0 sm:mx-4 w-64" />
                            <div className="sm:text-left">
                                <h2 className="text-xl font-bold">Manage your Discord server effectively.</h2>
                                <p className="text-base">
                                    ModCore is a powerful moderation bot that helps you keep your server safe and clean.
                                    It is jam-packed with features that help you manage your server effectively.
                                </p>
                            </div>
                        </div>

                        <div className="flex flex-col sm:flex-row-reverse items-center mb-8 bg-gray-900 py-6 px-2 m-4 max-w-xl rounded-lg shadow-lg">
                            <img src="https://placehold.co/600x400" alt="Feature 1" className="mb-4 sm:mb-0 sm:mx-4 w-64" />
                            <div className="sm:text-right">
                                <h2 className="text-xl font-bold">Keep track of great messages.</h2>
                                <p className="text-base">
                                    ModCore has a starboard feature that allows you to keep track of great messages.
                                    You can configure the starboard to your liking and keep your server active.
                                </p>
                            </div>
                        </div>

                        <div className="flex flex-col sm:flex-row items-center mb-8 bg-gray-900 py-6 px-2 m-4 max-w-xl rounded-lg shadow-lg">
                            <img src="https://placehold.co/600x400" alt="Feature 1" className="mb-4 sm:mb-0 sm:mx-4 w-64" />
                            <div className="sm:text-left">
                                <h2 className="text-xl font-bold">Keep out bad actors.</h2>
                                <p className="text-base">
                                    ModCore has a powerful moderation system that allows you to keep out bad actors.
                                    You can configure the moderation system to your liking and keep your server safe.
                                </p>
                            </div>
                        </div>

                        <div className="flex flex-col sm:flex-row-reverse items-center mb-8 bg-gray-900 py-6 px-2 m-4 max-w-xl rounded-lg shadow-lg">
                            <img src="https://placehold.co/600x400" alt="Feature 1" className="mb-4 sm:mb-0 sm:mx-4 w-64" />
                            <div className="sm:text-right">
                                <h2 className="text-xl font-bold">And more!</h2>
                                <p className="text-base">
                                    Add ModCore to your server and explore the features it has to offer.
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </MainLayout>
        </>
    );
}
