import { JSX } from "react";

export default function ModuleCard({link, name, icon, description} : {link: string, name: string, icon: JSX.Element, description: string})
{
    return <>
        <div
            className="bg-gray-900 p-5 h-32 flex flex-col justify-center cursor-pointer hover:bg-gray-700 hover:animate-pulse rounded-xl border bg-card text-card-foreground shadow"
            onClick={() =>
                (window.location.href = link)
            }
        >
            <h4 className="text-lg font-bold text-white inline-flex items-center py-1">
                <span className="pr-2">{icon}</span>
                {name}
            </h4>
            <p className="text-sm text-gray-400">
                {description}
            </p>
        </div>
    </>
}