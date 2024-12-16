import { Head, usePage } from "@inertiajs/react";
import MainLayout from "../Layouts/MainLayout.jsx";

export default function IndexPage(props) 
{
    const { apptitle, appdescription, dotnetVersion, authenticated, user, application } = props;

    if(authenticated)
        console.log(user);

    console.log(application);

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
                                {apptitle}
                            </span>
                        </h1>
                    </div>

                    <div className="bg-gray-900 py-6 pt-2 m-4 max-w-xl rounded-lg shadow-lg">
                        <p className="text-left text-lg max-w-xl mx-auto mt-8 px-4 sm:px-6 lg:px-8">
                            {appdescription}
                        </p>
                        <p className="text-center text-lg max-w-xl mx-auto mt-8 px-4 sm:px-6 lg:px-8">
                            Built with&nbsp;
                            <a href="https://dot.net" target="_blank" className="text-blue-400 hover:text-blue-200">
                                .NET {dotnetVersion}
                            </a>
                        </p>
                    </div>
                </div>
            </MainLayout>
        </>
    );
}
