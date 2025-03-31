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
import { useEffect, useState } from "react";
import { useForm, usePage } from "@inertiajs/react";
import DiscordChannelPicker from "../../../../Components/Forms/DiscordChannelPicker";
import { Input } from "../../../../Components/ui/input";
import ModCoreStarboard from "../../../../Types/DatabaseTypes/ModCoreStarboard";
import DiscordEmojiPicker from "../../../../Components/Forms/DiscordEmojiPicker";
import { DiscordChannel } from "../../../../Types/DiscordTypes/DiscordChannel";

interface ConfigureStarboardsProps {
    server: DiscordGuild;
    starboards: ModCoreStarboard[];
}

export default function Configure({
    user,
    server,
    starboards,
}: PagePropsWith<ConfigureStarboardsProps>) {
    let icon = server?.icon ? server.icon : null;
    if (icon) {
        let ext = icon.includes("a_") ? ".gif" : ".png";
        icon =
            "https://cdn.discordapp.com/icons/" + server.id + "/" + icon + ext;
    } else {
        icon = "https://cdn.discordapp.com/embed/avatars/0.png";
    }

    const { data, setData, post, processing, errors, clearErrors } = useForm({
        starboards: starboards,
    });

    useEffect(() => {
        setData("starboards", starboards);
    }, starboards);

    const addStarboard = () => {
        const newStarboards = [...(data.starboards as ModCoreStarboard[])];
        newStarboards.push({
            channel_id: "",
            enabled: true,
            emoji: "⭐",
            guild_id: server.id,
            id: undefined,
            minimum_reactions: 3,
        });
        setData("starboards", newStarboards);
    };

    const removeStarboard = (index: number) => {
        const newStarboards = [...(data.starboards as ModCoreStarboard[])];
        newStarboards.splice(index, 1);
        setData("starboards", newStarboards);
    };

    return (
        <>
            <DashboardLayout>
                <Button
                    variant={"outline"}
                    onClick={() =>
                        (window.location.href = `/dashboard/servers/${server.id}`)
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
                        {server.name}: Starboard Configuration
                    </h1>
                </div>
                <Card className="w-full bg-gray-900 p-6 rounded-lg shadow-lg">
                    <CardHeader>
                        <CardTitle className="text-2xl">
                            Starboard Configuration
                        </CardTitle>
                        <CardDescription>
                            Configure the starboard settings for this server.
                        </CardDescription>
                    </CardHeader>
                    <CardContent>
                        <form>
                            {(data.starboards as any)?.map(
                                (
                                    starboard: ModCoreStarboard,
                                    index: number
                                ) => {
                                    return (
                                        <div
                                            key={index}
                                            className="mb-8 p-4 border rounded-lg relative"
                                        >
                                            <div className="w-full flex items-center justify-between mb-2">
                                                <span className="text-lg font-semibold text-white">
                                                    Starboard (ID:{" "}
                                                    {starboard.id ?? "New"})
                                                </span>
                                                <Button
                                                    variant="destructive"
                                                    size="icon"
                                                    onClick={() => {
                                                        removeStarboard(index);
                                                    }}
                                                    className="absolute top-2 right-2"
                                                >
                                                    <svg
                                                        xmlns="http://www.w3.org/2000/svg"
                                                        width="24"
                                                        height="24"
                                                        viewBox="0 0 24 24"
                                                        fill="none"
                                                        stroke="currentColor"
                                                        strokeWidth="2"
                                                        strokeLinecap="round"
                                                        strokeLinejoin="round"
                                                        className="lucide lucide-trash"
                                                    >
                                                        <path d="M3 6h18" />
                                                        <path d="M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6" />
                                                        <path d="M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2" />
                                                    </svg>
                                                </Button>
                                            </div>
                                            <div className="flex flex-row items-center justify-between rounded-lg border p-4 mb-4">
                                                <div className="space-y-0.5">
                                                    <label
                                                        htmlFor={`enabled-${index}`}
                                                        className="text-base text-white"
                                                    >
                                                        Enable Starboard
                                                    </label>
                                                    <p className="text-sm text-gray-500">
                                                        Enable or disable the
                                                        starboard feature.
                                                    </p>
                                                </div>
                                                <Input
                                                    type="checkbox"
                                                    id={`enabled-${index}`}
                                                    name="enabled"
                                                    className="h-5 w-5 rounded-md border-gray-700 bg-gray-900 text-blue-500 focus:ring-blue-500"
                                                    checked={starboard.enabled}
                                                />
                                            </div>

                                            <div className="grid gap-4 mb-4">
                                                <div>
                                                    <label
                                                        htmlFor={`minimumReactions-${index}`}
                                                        className="block text-sm font-medium text-white"
                                                    >
                                                        Minimum Reactions
                                                    </label>
                                                    <Input
                                                        type="number"
                                                        id={`minimumReactions-${index}`}
                                                        name="minimumReactions"
                                                        className="mt-1 block w-full rounded-md border-gray-700 bg-gray-900 text-white shadow-sm focus:border-blue-500 focus:ring-blue-500"
                                                        value={
                                                            starboard.minimum_reactions
                                                        }
                                                    />
                                                    <p className="mt-2 text-sm text-gray-500">
                                                        The minimum number of
                                                        reactions required for a
                                                        message to be added to
                                                        the starboard.
                                                    </p>
                                                </div>

                                                <div>
                                                    <label
                                                        htmlFor={`emoji-${index}`}
                                                        className="block text-sm font-medium text-white"
                                                    >
                                                        Emoji
                                                    </label>
                                                    <DiscordEmojiPicker
                                                        label="Emoji"
                                                        onUpdate={(val) => {
                                                            const newStarboards = [...(data.starboards as ModCoreStarboard[])];
                                                            newStarboards[index].emoji = val || "⭐";
                                                            setData("starboards", newStarboards);
                                                        }}
                                                        value={starboard.emoji}
                                                    />
                                                    <p className="mt-2 text-sm text-gray-500">
                                                        The emoji used to
                                                        trigger the starboard.
                                                    </p>
                                                </div>

                                                <div>
                                                    <DiscordChannelPicker
                                                        label="Starboard Channel"
                                                        placeholder="Select a channel..."
                                                        value={starboard.channel_id}
                                                        onUpdate={(val) => {
                                                            const newStarboards = [...(data.starboards as any)];
                                                            newStarboards[index].channel_id = val;
                                                            setData("starboards", newStarboards);
                                                        }}
                                                        required
                                                    />
                                                    <p className="mt-2 text-sm text-gray-500">
                                                        The channel where
                                                        starboard messages will
                                                        be sent.
                                                    </p>
                                                </div>
                                            </div>
                                        </div>
                                    );
                                }
                            )}

                            <Button type="submit" disabled={processing}>
                                Save Changes
                            </Button>

                            <Button
                                type="button"
                                variant="secondary"
                                onClick={addStarboard}
                                className="ml-2 mb-4"
                            >
                                Add Starboard
                            </Button>
                        </form>
                    </CardContent>
                </Card>
            </DashboardLayout>
        </>
    );
}
