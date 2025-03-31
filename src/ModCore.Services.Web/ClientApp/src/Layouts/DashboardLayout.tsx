import MobileServerMenu from "@/Components/MobileServerMenu";
import Navbar from "@/Components/Navbar";
import Sidebar from "@/Components/Sidebar";
import { PropsWithChildren, useEffect, useState } from "react";
import { Toaster } from "../Components/ui/toaster";

export default function DashboardLayout({ children }: PropsWithChildren<{}>) {
    const [isMenuOpen, setIsMenuOpen] = useState(false);

    // useEffect that sets isMenuOpen to false when window resizes
    useEffect(() => {
        const handleResize = () => {
            if(isMenuOpen)
                setIsMenuOpen(false);
        };

        window.addEventListener("resize", handleResize);
        return () => window.removeEventListener("resize", handleResize);
    }, [isMenuOpen, setIsMenuOpen]);

    return (
        <>
            {/* Layout with a sidebar */}
            <div className="min-h-screen bg-gray-950 text-white">
                <div className="md:block hidden">
                    <Sidebar />
                </div>
                <div className="md:hidden block">
                    <MobileServerMenu isMenuOpen={isMenuOpen} setIsMenuOpen={setIsMenuOpen} />
                </div>
                <div className={`transition-opacity duration-300 ${isMenuOpen ? 'opacity-50 pointer-events-none' : 'opacity-100'}`}>
                    <header className="md:ml-12">
                        <Navbar isMenuOpen={isMenuOpen} setIsMenuOpen={setIsMenuOpen} />
                    </header>
                    <main className="md:ml-12 py-10 px-6 sm:px-10 mt-10 md:mt-0">
                        <div className="container mx-auto relative">
                            {children}
                        </div>
                    </main>
                </div>
            </div>
            <Toaster />
        </>
    );

}
