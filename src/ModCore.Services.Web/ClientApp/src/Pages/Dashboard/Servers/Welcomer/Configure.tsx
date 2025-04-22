import { Head, router, useForm, usePage } from "@inertiajs/react";
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
import DiscordChannelPicker from "@/Components/Forms/DiscordChannelPicker";
import { Button } from "@/Components/ui/button";
import {
    Card,
    CardContent,
    CardDescription,
    CardFooter,
    CardHeader,
    CardTitle,
} from "@/Components/ui/card";

type ManagePageProps = {
    server: DiscordGuild;
    permissions: string[];
    welcomeSettings: {
        enabled: boolean;
        channel_id?: string;
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

    if (!authenticated) router.visit("/login");

    if (!hasPermissionsFromString(server.permissions, "MANAGE_GUILD")) {
        router.visit("/login");
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
        channel_id: welcomeSettings.channel_id ?? String(0),
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

    function setChannelId(channelId?: string) {
        setData("channel_id", channelId ?? "");
    }

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
                    <div className="pb-20">
                        {/* Back to Overview link */}
                        <Button variant={"outline"} onClick={() => router.visit(`/dashboard/servers/${server.id}`)} className="mb-4">
                            Back to Overview
                        </Button>
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
                        
                        {/* Update the flex layout structure */}
                        <div className="mt-6 md:flex md:space-x-6 relative"
                            style={{alignItems: "start"}}
                        >
                            {/* Left column */}
                            <div className="w-full md:w-1/2 mb-6 md:mb-0">
                                <Card className="w-full bg-gray-900 p-6 rounded-lg shadow-lg">
                                    <CardHeader>
                                        <CardTitle className="text-2xl">
                                            Welcome Message Configuration
                                        </CardTitle>
                                        <CardDescription>
                                            Configure the welcome message that is sent to new members when they join the server.
                                        </CardDescription>
                                    </CardHeader>
                                    <CardContent>
                                        <div className="space-y-6">
                                            <ToggleSwitch
                                                enabled={data.enabled}
                                                onUpdate={setEnabled}
                                                label="Enabled"
                                            />
                                            <DiscordChannelPicker
                                                label="Welcome Channel"
                                                placeholder="Select a channel..."
                                                value={data.channel_id}
                                                onUpdate={setChannelId}
                                                required
                                            />
                                            <TextArea
                                                label="Message Content"
                                                rows={4}
                                                placeholder="Welcome, {{mention}}!"
                                                value={data.message_payload.content}
                                                onUpdate={setContent}
                                            />
                                        </div>
                                    </CardContent>
                                    <CardFooter>
                                        <Button
                                            className="w-full"
                                            onClick={() =>
                                                post(
                                                    `/dashboard/servers/${server.id}/welcome`
                                                )
                                            }
                                            disabled={processing}
                                        >
                                            {processing ? "Saving..." : "Save"}
                                        </Button>
                                    </CardFooter>
                                </Card>
                                <Card className="w-full bg-gray-900 p-6 rounded-lg shadow-lg mt-6">
                                    <CardHeader>
                                        <CardTitle className="text-2xl">
                                            Embeds
                                        </CardTitle>
                                        <CardDescription>
                                            Configure the embeds that are sent with the welcome message.
                                        </CardDescription>
                                    </CardHeader>
                                    <CardContent>
                                        <div className="space-y-4">
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
                                        </div>
                                        {(data.message_payload.embeds?.length ?? 0) < 3 && (
                                            <Button
                                                className="w-full"
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
                                            </Button>
                                        )}
                                    </CardContent>
                                </Card>
                            </div>
                            
                            {/* Right column - properly sticky */}
                            <div className="w-full md:w-1/2 md:sticky md:top-10 self-start">
                                <Card className="w-full bg-gray-900 p-6 rounded-lg shadow-lg">
                                    <CardHeader>
                                        <CardTitle className="text-2xl">
                                            Available Placeholders
                                        </CardTitle>
                                        <CardDescription>
                                            Use these placeholders to customize your welcome message.
                                        </CardDescription>
                                    </CardHeader>
                                    <CardContent>
                                        <div className="flex flex-wrap gap-2">
                                            {availablePlaceholders.map((placeholder) => (
                                                <code
                                                    className="text-blue-400 bg-gray-800 p-1 rounded-md text-sm"
                                                    key={placeholder}
                                                >
                                                    {'{'}{'{'}{placeholder}{'}'}{'}'}
                                                </code>
                                            ))}
                                        </div>
                                    </CardContent>
                                </Card>
                                <DiscordMessageVisualizerProxy
                                    content={data.message_payload.content}
                                    embeds={
                                        data.message_payload.embeds ?? undefined
                                    }
                                    className="bg-gray-900 p-6 mt-6 rounded-xl border bg-card text-card-foreground shadow"
                                />
                            </div>
                        </div>
                    </div>
                )}
            </DashboardLayout>
        </>
    );
}
