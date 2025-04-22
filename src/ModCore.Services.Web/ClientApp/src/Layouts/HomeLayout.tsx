import { Link, usePage } from '@inertiajs/react';
import React, { PropsWithChildren } from 'react';

export default function HomeLayout({ title, children } : PropsWithChildren<{title?: string }>) {
    return (
        <div className="min-h-screen flex flex-col bg-gray-950 text-white">
            <main className="mx-auto py-4 sm:px-8 flex-grow w-full max-w-6xl">
                {children}
            </main>
        </div>
    );
};