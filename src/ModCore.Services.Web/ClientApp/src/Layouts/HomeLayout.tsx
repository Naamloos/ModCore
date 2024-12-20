import { Link, usePage } from '@inertiajs/react';
import React, { PropsWithChildren } from 'react';

export default function HomeLayout({ title, children } : PropsWithChildren<{title?: string }>) {
    const { user, application } : {user: any, application: any} = usePage().props as any;
    const copyrightYear = new Date().getFullYear();
    const authenticated = user != null;

    return (
        <div className="min-h-screen flex flex-col bg-gray-950 text-white">
            <main className="mx-auto py-4 sm:px-8 flex-grow w-full max-w-6xl">
                {children}
            </main>
        </div>
    );
};