import { Head, usePage } from "@inertiajs/react";
import { Welcome } from "../components/Welcome/Welcome";
import MainLayout from "../Layouts/MainLayout.jsx";

export default function IndexPage(props) 
{
    const { apptitle, appdescription, dotnetversion } = props;

    return (
        <>
            <MainLayout>
                <Head title="Welcome" />
                <h1 className="text-center mt-24 text-7xl font-extrabold tracking-tight">
                    Welcome to <br />
                    <span className="bg-gradient-to-t from-blue-500 to-cyan-200 bg-clip-text text-transparent">
                        {apptitle}
                    </span>
                </h1>
                <p className="text-center text-lg max-w-xl mx-auto mt-8">
                    {appdescription}
                </p>
                <p className="text-center text-lg max-w-xl mx-auto mt-8">
                    Built with&nbsp;
                    <a href="https://dot.net" target="_blank" className="text-blue-400 hover:text-blue-200">
                        .NET {dotnetversion}
                    </a>
                </p>
            </MainLayout>
        </>
    );
}
