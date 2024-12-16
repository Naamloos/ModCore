import { Link } from '@inertiajs/react';
import React from 'react';

export default function MainLayout({ children })
{
    const copyrightYear = new Date().getFullYear();

    return (
        <div className="min-h-screen flex flex-col bg-gray-900 text-white">
            <header className="bg-gray-800 p-4 shadow-md">
                <div className="container mx-auto flex justify-between items-center">
                    <h1 className="text-2xl font-bold">ModCore</h1>
                    <nav>
                        <Link href="/" className="text-gray-300 hover:text-white mx-2">Home</Link>
                        <Link href="/features" className="text-gray-300 hover:text-white mx-2">Features</Link>
                        <Link href="/login" className="text-gray-300 hover:text-white mx-2">Dashboard</Link>
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