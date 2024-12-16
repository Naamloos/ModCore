import { Link, usePage } from '@inertiajs/react';
import React from 'react';
import Logo from '../Resources/logo.png';

export default function MainLayout({ children }) {
    const { authenticated, user } = usePage().props;
    const copyrightYear = new Date().getFullYear();

    return (
        <div className="min-h-screen flex flex-col bg-gray-900 text-white">
            <header className="bg-gray-800 p-4 shadow-md">
                <div className="container mx-auto flex flex-wrap justify-between items-center">
                    <div className="flex items-center">
                        <img src={Logo} className="h-8 w-8 mr-2" />
                        <h1 className="text-2xl font-bold">ModCore</h1>
                    </div>
                    <nav className="grid grid-flow-col gap-2 items-center mt-2 sm:mt-0">
                        {authenticated ? <>
                            <span>Logged in as: <span className="text-green-200">{user.username}</span></span>
                            <img src={user.avatar} className="h-10 w-10 rounded-full mr-2" /> 
                            <a href="/login" className="bg-[#089fdf] text-white px-4 py-2 rounded hover:bg-blue-600 transition duration-300">
                                Dashboard
                            </a>
                            <a href="/logout" className="bg-red-700 text-white px-4 py-2 rounded hover:bg-red-600 transition duration-300">
                                Log Out
                            </a>
                        </>: <>
                            <a href="/login" className="bg-[#089fdf] text-white px-4 py-2 rounded hover:bg-blue-600 transition duration-300">
                                Log In
                            </a>
                        </>}
                    </nav>
                </div>
            </header>
            <main className="container mx-auto p-4 flex-grow">
                {children}
            </main>
            <footer className="bg-gray-800 p-4">
                <div className="container mx-auto text-center">
                    <p className="text-gray-400">&copy; {copyrightYear} ModCore. All rights reserved.</p>
                </div>
            </footer>
        </div>
    );
};