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
import { ModCoreGuild } from "../../../../Types/DatabaseTypes/ModCoreGuild";

interface ConfigureLevelingProps {
    server: DiscordGuild;
    databaseServer: ModCoreGuild;
}

export default function Configure({
    server,
    databaseServer,
}: PagePropsWith<ConfigureLevelingProps>) {
    const [state, setState] = useState(
        databaseServer.embed_message_links_state
    );

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
            state,
        });
    };

    const { data, setData, post, processing, errors } = useForm({
        state,
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
                        {server.name}: Jump Link Preview Configuration
                    </h1>
                </div>
                <Card className="w-full bg-gray-900 p-6 rounded-lg shadow-lg">
                    <CardHeader>
                        <CardTitle className="text-2xl">
                            Jump Link Preview Configuration
                        </CardTitle>
                        <CardDescription>
                            Configure the jump link preview settings for this
                            server.
                            <br />
                            This setting controls if, and how jump links are
                            embedded in messages.
                            <br />
                            <br />
                            <strong>Disabled</strong>: No jump link previews
                            will be shown.
                            <br />
                            <strong>Prefixed</strong>: Jump link previews will
                            be shown, but only if the link starts with an
                            exclamation mark.
                            <br />
                            <strong>Always</strong>: Jump link previews will be
                            shown, regardless of the message content.
                        </CardDescription>
                    </CardHeader>
                    <CardContent>
                        <form className="space-y-8" onSubmit={handleSubmit}>
                            <div>
                                <h3 className="text-lg font-medium">
                                    Embed Message Links
                                </h3>
                                <p className="text-sm text-muted-foreground">
                                    Choose when to embed message links.
                                </p>
                                <select
                                    id="embed-message-links"
                                    className="w-full p-2 border rounded-md bg-gray-800 text-white"
                                    value={state}
                                    onChange={(e) =>
                                        setState(parseInt(e.target.value))
                                    }
                                >
                                    <option value={0}>Disabled</option>
                                    <option value={1}>Prefixed</option>
                                    <option value={2}>Always</option>
                                </select>
                            </div>

                            <Button type="submit">Submit</Button>
                        </form>
                    </CardContent>
                </Card>
            </DashboardLayout>
        </>
    );
}
