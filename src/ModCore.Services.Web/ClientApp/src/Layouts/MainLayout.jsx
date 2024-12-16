import { Link, usePage } from '@inertiajs/react';
import React from 'react';

export default function MainLayout({ title = "", children }) {
    const { authenticated, user, application } = usePage().props;
    const copyrightYear = new Date().getFullYear();

    return (
        <div className="min-h-screen flex flex-col bg-gray-800 text-white">
            <header className="bg-gray-900 p-4 shadow-md">
                <div className="container mx-auto flex flex-wrap justify-between items-center">
                    <div className="flex items-center">
                        <a href="/" className="flex items-center">
                            <img src={`https://cdn.discordapp.com/app-icons/${application.id}/${application.icon}.png`} className="h-8 w-8 mr-2" />
                            <h1 className="text-2xl font-bold">ModCore {(title != '') && (' - ' + title)}</h1>
                        </a>
                    </div>
                    <nav className="grid grid-flow-col gap-2 items-center mt-2 sm:mt-0">
                        {authenticated ? <>
                            <span>Logged in as: <span className="text-green-400">{user.username}</span></span>
                            <img src={user.avatar} className="h-10 w-10 rounded-full mr-2" /> 
                            <a href="/dashboard" className="bg-[#7289da] text-white px-4 py-2 rounded hover:bg-[#677bc4] transition duration-300">
                                Dashboard
                            </a>
                            <a href="/logout" className="bg-red-400 text-white px-4 py-2 rounded hover:bg-red-300 transition duration-300">
                                Log Out
                            </a>
                        </> : <>
                            <a href="/login" className="bg-blue-400 text-white px-4 py-2 rounded hover:bg-blue-300 transition duration-300">
                                Log In
                            </a>
                        </>}
                    </nav>
                </div>
            </header>
            <main className="container mx-auto p-4 flex-grow">
                {children}
            </main>
            <footer className="bg-gray-900 p-4">
                <div className="container mx-auto text-center">
                    <p className="text-gray-400">&copy; {copyrightYear} ModCore. All rights reserved.</p>
                </div>
            </footer>
        </div>
    );
};