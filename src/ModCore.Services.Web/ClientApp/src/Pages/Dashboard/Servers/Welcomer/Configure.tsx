import { Head, useForm, usePage } from "@inertiajs/react";
import HomeLayout from "@/Layouts/HomeLayout.js";
import { PagePropsWith } from "@/Types/PageProps";
import { DiscordGuild } from "@/Types/DiscordTypes/DiscordGuild";
import { ModCoreGuild } from "@/Types/DatabaseTypes/ModCoreGuild";
import DashboardLayout from "@/Layouts/DashboardLayout";
import { hasPermissionsFromString } from "@/Types/DiscordTypes/DiscordPermission";
import { WelcomeMessageJson } from "@/Types/DatabaseTypes/ModCoreWelcomeSettings";
import FakeDiscordMessage from "@/Components/FakeDiscordMessage";
import EmbedForm from "./Partials/EmbedForm";
import ToggleSwitch from "@/Components/Forms/ToggleSwitch";
import TextArea from "@/Components/Forms/TextArea";
import { useEffect, useRef, useState } from "react";
import DiscordMessageVisualizerProxy from "./Partials/DiscordMessageVisualizerProxy";

type ManagePageProps = {
    server: DiscordGuild;
    permissions: string[];
    welcomeSettings: {
        enabled: boolean;
        channel_id: string;
        welcome_message_json: string;
    };
};

const availablePlaceholders = [
    "username",
    "mention",
    "userid",
    "guildname",
    "channelname",
    "membercount",
    "owner-username",
    "guild-icon-url",
    "avatar",
    "channel-count",
    "role-count"
];

export default function Configure({
    user,
    server,
    welcomeSettings
}: PagePropsWith<ManagePageProps>) {
    const authenticated = user != null;

    if (!authenticated) window.location.href = "/login";

    if (!hasPermissionsFromString(server.permissions, "MANAGE_GUILD")) {
        window.location.href = "/login";
    }

    let icon = server?.icon ? server.icon : null;
    if (icon) {
        let ext = icon.includes("a_") ? ".gif" : ".png";
        icon =
            "https://cdn.discordapp.com/icons/" + server.id + "/" + icon + ext;
    } else {
        icon = "https://cdn.discordapp.com/embed/avatars/0.png";
    }

    console.log(welcomeSettings);

    const originalWelcomeJson = JSON.parse(
        welcomeSettings.welcome_message_json ?? "{}"
    ) as WelcomeMessageJson;

    // inertia UseForm:
    const { data, setData, post, processing, errors } = useForm({
        message_payload: originalWelcomeJson,
        enabled: welcomeSettings.enabled ?? false,
        channel_id: welcomeSettings.channel_id ?? 0,
    });

    // Function to update the messagePayload
    function setContent(content: string) {
        setData("message_payload", {
            ...data.message_payload,
            content,
        });
    }

    function setEnabled(enabled: boolean) {
        setData("enabled", enabled);
    }

    function setChannelId(channelId: string) {
        setData("channel_id", BigInt(channelId));
    }

    const fakeDiscordMessageRef = useRef<HTMLDivElement>(null);
    const formBoundRef = useRef<HTMLDivElement>(null);
    const [minScroll, setMinScroll] = useState<number | null>(null);

    useEffect(() => {
        const handleScroll = () => {
            if (!fakeDiscordMessageRef.current || !formBoundRef.current) return;

            const formRect = formBoundRef.current.getBoundingClientRect();
            const fakeMessageRect = fakeDiscordMessageRef.current.getBoundingClientRect();

            if (fakeMessageRect.top < 0 && minScroll === null) {
                setMinScroll(window.scrollY);
            }

            if (minScroll !== null) {
                if (window.scrollY >= minScroll) {
                    fakeDiscordMessageRef.current.style.position = "fixed";
                    fakeDiscordMessageRef.current.style.top = "0";
                    fakeDiscordMessageRef.current.style.left = `${formRect.right + 16}px`;
                    fakeDiscordMessageRef.current.style.width = `${window.innerWidth - formRect.right - 32}px`;
                } else {
                    fakeDiscordMessageRef.current.style.position = "";
                    fakeDiscordMessageRef.current.style.top = "";
                    fakeDiscordMessageRef.current.style.left = "";
                    fakeDiscordMessageRef.current.style.width = "";
                }
            }
        };

        const handleResize = () => {
            if (!fakeDiscordMessageRef.current || !formBoundRef.current) return;

            const formRect = formBoundRef.current.getBoundingClientRect();
            fakeDiscordMessageRef.current.style.width = `${window.innerWidth - formRect.right - 32}px`;

            handleScroll();
        };

        window.addEventListener("scroll", handleScroll);
        window.addEventListener("resize", handleResize);

        return () => {
            window.removeEventListener("scroll", handleScroll);
            window.removeEventListener("resize", handleResize);
        };
    }, [minScroll]);

    return (
        <>
            <Head title="Welcome Message Configuration" />

            <DashboardLayout>
                {server == null && (
                    <div className="text-center text-white">
                        <h1 className="text-3xl font-extrabold tracking-tight">
                            ModCore is not in this server!
                        </h1>
                        <p className="mt-4">
                            Please contact the server owner to add ModCore.
                        </p>
                        <p className="mt-4">
                            <a
                                href="/dashboard/servers"
                                className="text-blue-400 hover:text-blue-300"
                            >
                                Back to Servers
                            </a>
                        </p>
                    </div>
                )}
                {server && (
                    <>
                        {/* Back to Overview link */}
                        <a
                            href={`/dashboard/servers/${server.id}`}
                            className="text-blue-400 hover:text-blue-300"
                        >
                            &lt;&lt; Back to Overview
                        </a>
                        <div className="md:flex items-center mb-4">
                            <img
                                src={icon}
                                className="inline-block h-16 w-16 rounded-full"
                                alt="User Avatar"
                            />
                            <h1 className="text-3xl font-extrabold tracking-tight text-white ml-4">
                                {server.name}: Welcome Message
                            </h1>
                        </div>
                        <div className="grid md:grid-cols-2 gap-4 relative">
                            <div>
                                <div 
                                    className="bg-gray-800 p-4 rounded-lg shadow-md mt-4"
                                    ref={formBoundRef}
                                >
                                    <div className="mb-4">
                                        <ToggleSwitch
                                            enabled={data.enabled}
                                            onUpdate={setEnabled}
                                            label="Enabled"
                                        />
                                        <TextArea
                                            label="Message Content"
                                            rows={4}
                                            placeholder="Welcome, {{mention}}!"
                                            value={data.message_payload.content}
                                            onUpdate={setContent}
                                        />
                                    </div>
                                    <div className="mt-4 mb-4">
                                        <button
                                            className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
                                            onClick={() =>
                                                post(
                                                    `/dashboard/servers/${server.id}/welcome`
                                                )
                                            }
                                            disabled={processing}
                                        >
                                            {processing ? "Saving..." : "Save"}
                                        </button>
                                    </div>
                                </div>
                                <div className="bg-gray-800 p-4 rounded-lg shadow-md mt-4">
                                    <h2 className="text-xl font-bold text-white mb-4">
                                        Embeds
                                    </h2>
                                    {data.message_payload.embeds?.map(
                                        (embed, index) => (
                                            <EmbedForm
                                                key={index}
                                                index={index}
                                                embed={embed}
                                                update={(newEmbed) => {
                                                    const newEmbeds = [
                                                        ...data.message_payload
                                                            .embeds,
                                                    ];
                                                    newEmbeds[index] = newEmbed;
                                                    setData("message_payload", {
                                                        ...data.message_payload,
                                                        embeds: newEmbeds,
                                                    });
                                                }}
                                                remove={() => {
                                                    const newEmbeds = [
                                                        ...data.message_payload
                                                            .embeds,
                                                    ];
                                                    newEmbeds.splice(index, 1);
                                                    setData("message_payload", {
                                                        ...data.message_payload,
                                                        embeds: newEmbeds,
                                                    });
                                                }}
                                            />
                                        )
                                    )}
                                    {(data.message_payload.embeds?.length ?? 0) < 3 && (
                                        <button
                                            className="bg-green-500 hover:bg-green-700 text-white font-bold py-2 px-4 rounded"
                                            onClick={() => {
                                                const newEmbeds = [
                                                    ...(data.message_payload
                                                        .embeds || []),
                                                    {},
                                                ];
                                                setData("message_payload", {
                                                    ...data.message_payload,
                                                    embeds: newEmbeds,
                                                });
                                            }}
                                        >
                                            Add Embed
                                        </button>
                                    )}
                                </div>
                            </div>
                            <div ref={fakeDiscordMessageRef}>
                                <div className="bg-gray-800 p-4 rounded-lg shadow-md mt-4">
                                    Available Placeholders:
                                    <br/>
                                    {availablePlaceholders.map((placeholder) => (
                                        <code className="ml-2 text-blue-600 bg-gray-900 p-0.5 rounded-md text-sm" key={placeholder}>
                                            {'{'}{'{'}{placeholder}{'}'}{'}'}
                                        </code>
                                    ))}
                                </div>
                                <DiscordMessageVisualizerProxy
                                    content={data.message_payload.content}
                                    embeds={
                                        data.message_payload.embeds ?? undefined
                                    }
                                    className="h-fit w-full mt-4 sticky top-0"
                                />
                            </div>
                        </div>
                    </>
                )}
            </DashboardLayout>
        </>
    );
}
