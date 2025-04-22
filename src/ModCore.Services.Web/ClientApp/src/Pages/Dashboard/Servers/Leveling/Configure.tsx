import DashboardLayout from "@/Layouts/DashboardLayout";
import { DiscordGuild } from "@/Types/DiscordTypes/DiscordGuild";
import { PagePropsWith } from "@/Types/PageProps";
import { Button } from "@/Components/ui/button";
import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from "@/Components/ui/card";
import { Input } from "@/Components/ui/input";
import { Switch } from "@/Components/ui/switch";
import { useState } from "react";
import DiscordChannelPicker from "../../../../Components/Forms/DiscordChannelPicker";
import { router, useForm } from "@inertiajs/react";
import ModCoreLevelSettings from "../../../../Types/DatabaseTypes/ModCoreLevelSettings";

interface ConfigureLevelingProps {
    server: DiscordGuild;
    settings: ModCoreLevelSettings;
}

export default function Configure({
    server,
    settings,
}: PagePropsWith<ConfigureLevelingProps>) {
    const [enabled, setEnabled] = useState(settings.enabled);
    const [messagesEnabled, setMessagesEnabled] = useState(settings.messages_enabled);
    const [redirectMessages, setRedirectMessages] = useState(settings.redirect_messages);
    const [channelId, setChannelId] = useState<string | undefined>(settings.message_channel_id.toString());

    let icon = server?.icon ? server.icon : null;
    if (icon) {
        let ext = icon.includes("a_") ? ".gif" : ".png";
        icon =
            "https://cdn.discordapp.com/icons/" + server.id + "/" + icon + ext;
    } else {
        icon = "https://cdn.discordapp.com/embed/avatars/0.png";
    }

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        // Handle form submission here
        console.log({
            enabled,
            messagesEnabled,
            redirectMessages,
            channelId,
        });
    };

    const { data, setData, post, processing, errors } = useForm({
        enabled,
        messagesEnabled,
        redirectMessages,
        channelId,
    });

    return (
        <>
            <DashboardLayout>
                <Button
                    variant={"outline"}
                    onClick={() =>
                        router.visit(`/dashboard/servers/${server.id}`)
                    }
                    className="mb-4"
                >
                    Back to Overview
                </Button>
                <div className="md:flex items-center mb-4">
                    <img
                        src={icon}
                        className="inline-block h-16 w-16 rounded-full"
                        alt="User Avatar"
                    />
                    <h1 className="text-3xl font-extrabold tracking-tight text-white ml-4">
                        {server.name}: Leveling Configuration
                    </h1>
                </div>
                <Card className="w-full bg-gray-900 p-6 rounded-lg shadow-lg">
                    <CardHeader>
                        <CardTitle className="text-2xl">
                            Leveling Configuration
                        </CardTitle>
                        <CardDescription>
                            Configure the leveling settings for this server.
                        </CardDescription>
                    </CardHeader>
                    <CardContent>
                        <form className="space-y-8" onSubmit={handleSubmit}>
                            <div className="flex flex-row items-center justify-between rounded-lg border p-4">
                                <div className="space-y-0.5">
                                    <label
                                        htmlFor="enabled"
                                        className="text-base text-white"
                                    >
                                        Enable Leveling
                                    </label>
                                    <p className="text-sm text-gray-500 dark:text-gray-400">
                                        Enable or disable the leveling feature.
                                    </p>
                                </div>
                                <Switch
                                    id="enabled"
                                    checked={enabled}
                                    onCheckedChange={setEnabled}
                                />
                            </div>

                            <div className="flex flex-row items-center justify-between rounded-lg border p-4">
                                <div className="space-y-0.5">
                                    <label
                                        htmlFor="messagesEnabled"
                                        className="text-base text-white"
                                    >
                                        Enable Leveling Messages
                                    </label>
                                    <p className="text-sm text-gray-500 dark:text-gray-400">
                                        Enable or disable level up messages.
                                    </p>
                                </div>
                                <Switch
                                    id="messagesEnabled"
                                    checked={messagesEnabled}
                                    onCheckedChange={setMessagesEnabled}
                                />
                            </div>

                            <div className="flex flex-row items-center justify-between rounded-lg border p-4">
                                <div className="space-y-0.5">
                                    <label
                                        htmlFor="redirectMessages"
                                        className="text-base text-white"
                                    >
                                        Redirect Messages
                                    </label>
                                    <p className="text-sm text-gray-500 dark:text-gray-400">
                                        Redirect level up messages to a specific
                                        channel.
                                    </p>
                                </div>
                                <Switch
                                    id="redirectMessages"
                                    checked={redirectMessages}
                                    onCheckedChange={setRedirectMessages}
                                />
                            </div>

                            <div>
                                <div className="mt-1">
                                    <DiscordChannelPicker
                                        label="Message Redirect Channel"
                                        placeholder="Select a channel..."
                                        value={data.channelId}
                                        onUpdate={setChannelId}
                                    />
                                </div>
                                <p className="mt-2 text-sm text-gray-500 dark:text-gray-400">
                                    The channel where level up messages will be
                                    sent.
                                </p>
                            </div>

                            <Button type="submit">Submit</Button>
                        </form>
                    </CardContent>
                </Card>
            </DashboardLayout>
        </>
    );
}
