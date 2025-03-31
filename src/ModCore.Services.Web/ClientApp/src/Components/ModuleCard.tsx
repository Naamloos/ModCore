import { JSX } from "react";
import { useToast } from "@/hooks/use-toast";

export default function ModuleCard({link, name, icon, description, done} : {link: string, name: string, icon: JSX.Element, description: string, done: boolean})
{
    const { toast } = useToast()

    return <>
        <div
            className={`bg-gray-900 p-5 h-32 flex flex-col justify-center cursor-pointer hover:bg-gray-700 hover:animate-pulse rounded-xl border bg-card text-card-foreground shadow ${(!done) ? 'opacity-50 cursor-not-allowed' : ''}`}
            onClick={() =>
                {
                    if(done)
                    {
                        window.location.href = link
                    }
                    else
                    {
                        toast({
                            title: "Module In Development",
                            description: "This module is still in development! Please be patient.",
                        })
                    }
                }
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