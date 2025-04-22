import { Head, router, usePage } from "@inertiajs/react";
import HomeLayout from "@/Layouts/HomeLayout.js";
import { IconBrandGithub } from "@tabler/icons-react";
import { PagePropsWith } from "@/Types/PageProps";
import SignInWithDiscord from "@/Components/SignInWithDiscord";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/Components/ui/card";
import { Separator } from "@/Components/ui/separator";
import LogoImage from "@/Assets/logo.png";

type IndexPageProps = {
    dotnetVersion: string;
};

export default function IndexPage({ dotnetVersion, application }: PagePropsWith<IndexPageProps>) {
    return (
        <>
            <HomeLayout>
                <Head title="Welcome" />
                <div className="container mx-auto px-4 py-8">
                    {/* Hero Section */}
                    <section className="text-center">
                        <div className="flex flex-col items-center justify-center">
                            <img
                                src={LogoImage}
                                alt="Logo"
                                className="h-24 w-24 md:h-36 md:w-36"
                                style={{ animation: 'shake 0.5s', animationIterationCount: 'infinite', animationPlayState: 'paused' }}
                                onMouseEnter={(e) => {
                                    (e.target as HTMLImageElement).style.animationPlayState = 'running';
                                }}
                                onMouseLeave={(e) => {
                                    (e.target as HTMLImageElement).style.animationPlayState = 'paused';
                                }}
                            />
                            <h1 className="text-4xl md:text-5xl font-bold mt-4 bg-gradient-to-r from-[#089fe0] to-[#00d4ff] text-transparent bg-clip-text">
                                Welcome to <span>ModCore</span>
                            </h1>
                            <p className="text-gray-300 mt-2 text-lg">
                                Your assistant for Discord server moderation and management.
                            </p>
                            <style>
                                {`
                                @keyframes shake {
                                    0% { transform: translate(1px, 1px) rotate(0deg); }
                                    10% { transform: translate(-1px, -2px) rotate(-1deg); }
                                    20% { transform: translate(-3px, 0px) rotate(1deg); }
                                    30% { transform: translate(3px, 2px) rotate(0deg); }
                                    40% { transform: translate(1px, -1px) rotate(1deg); }
                                    50% { transform: translate(-1px, 2px) rotate(-1deg); }
                                    60% { transform: translate(-3px, 1px) rotate(0deg); }
                                    70% { transform: translate(3px, 1px) rotate(-1deg); }
                                    80% { transform: translate(-1px, -1px) rotate(1deg); }
                                    90% { transform: translate(1px, 2px) rotate(0deg); }
                                    100% { transform: translate(1px, -2px) rotate(-1deg); }
                                }
                                `}
                            </style>
                        </div>

                        <Card className="mt-8 w-full max-w-3xl mx-auto bg-gradient-to-br from-gray-800 to-gray-900 text-white shadow-xl rounded-lg">
                            <CardHeader>
                                <CardTitle className="text-2xl font-semibold">About ModCore</CardTitle>
                                <CardDescription className="text-gray-400">
                                    Learn more about what ModCore can do for your server.
                                </CardDescription>
                            </CardHeader>
                            <CardContent>
                                <p className="text-left text-sm md:text-base text-gray-300">
                                    ModCore is designed to simplify Discord server management with a range of features that make moderation a breeze.
                                </p>
                                <Separator className="my-4 bg-gray-700" />
                                <div className="flex items-center justify-center space-x-4">
                                    <p className="text-gray-400 text-xs md:text-sm">
                                        Built with .NET {dotnetVersion}
                                    </p>
                                    <a
                                        href="https://github.com/Naamloos/ModCore/"
                                        target="_blank"
                                        className="text-blue-500 hover:text-blue-400 transition-transform transform hover:scale-110"
                                    >
                                        <IconBrandGithub size={20} />
                                    </a>
                                </div>
                                <div className="mt-4">
                                    <SignInWithDiscord onClick={() => {
                                        router.visit("/login");
                                    }} />
                                </div>
                            </CardContent>
                        </Card>
                    </section>

                    {/* Features Section */}
                    <section className="mt-12">
                        <h2 className="text-3xl font-semibold text-center mb-8 bg-gradient-to-r from-[#089fe0] to-[#00d4ff] text-transparent bg-clip-text">
                            Key Features
                        </h2>
                        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                            {/* Feature 1 */}
                            <Card className="bg-gradient-to-br from-gray-800 to-gray-900 text-white shadow-lg rounded-lg hover:shadow-2xl transition-shadow">
                                <CardHeader>
                                    <CardTitle className="text-lg font-semibold">Effective Server Management</CardTitle>
                                </CardHeader>
                                <CardContent>
                                    <img
                                        src="https://placehold.co/600x400"
                                        alt="Feature 1"
                                        className="mb-4 rounded-md"
                                    />
                                    <p className="text-sm md:text-base text-gray-300">
                                        ModCore helps you keep your server safe and clean with powerful moderation tools.
                                    </p>
                                </CardContent>
                            </Card>

                            {/* Feature 2 */}
                            <Card className="bg-gradient-to-br from-gray-800 to-gray-900 text-white shadow-lg rounded-lg hover:shadow-2xl transition-shadow">
                                <CardHeader>
                                    <CardTitle className="text-lg font-semibold">Starboard for Great Messages</CardTitle>
                                </CardHeader>
                                <CardContent>
                                    <img
                                        src="https://placehold.co/600x400"
                                        alt="Feature 2"
                                        className="mb-4 rounded-md"
                                    />
                                    <p className="text-sm md:text-base text-gray-300">
                                        Keep track of great messages with a customizable starboard feature.
                                    </p>
                                </CardContent>
                            </Card>

                            {/* Feature 3 */}
                            <Card className="bg-gradient-to-br from-gray-800 to-gray-900 text-white shadow-lg rounded-lg hover:shadow-2xl transition-shadow">
                                <CardHeader>
                                    <CardTitle className="text-lg font-semibold">Powerful Moderation System</CardTitle>
                                </CardHeader>
                                <CardContent>
                                    <img
                                        src="https://placehold.co/600x400"
                                        alt="Feature 3"
                                        className="mb-4 rounded-md"
                                    />
                                    <p className="text-sm md:text-base text-gray-300">
                                        Keep out bad actors with a robust moderation system.
                                    </p>
                                </CardContent>
                            </Card>
                        </div>
                    </section>
                </div>
            </HomeLayout>
        </>
    );
}
