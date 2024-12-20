import { Head, useForm, usePage } from "@inertiajs/react";
import HomeLayout from "@/Layouts/HomeLayout.js";
import { PagePropsWith } from "@/Types/PageProps";
import { DiscordGuild } from "@/Types/DiscordTypes/DiscordGuild";
import { ModCoreGuild } from "@/Types/DatabaseTypes/ModCoreGuild";
import DashboardLayout from "@/Layouts/DashboardLayout";
import {
    IconAlertSquareRounded,
    IconBook,
    IconChartLine,
    IconHammer,
    IconHandMove,
    IconHandStop,
    IconJumpRope,
    IconMoodAngry,
    IconMoon,
    IconNotebook,
    IconRobot,
    IconStar,
    IconTag,
    IconTicket,
    IconTree,
} from "@tabler/icons-react";
import { hasPermissionsFromString } from "@/Types/DiscordTypes/DiscordPermission";
import ModuleCard from "@/Components/ModuleCard";
import { WelcomeMessageJson } from "@/Types/DatabaseTypes/ModCoreWelcomeSettings";
import FakeDiscordMessage from "@/Components/FakeDiscordMessage";

type ManagePageProps = {
    server: DiscordGuild;
    permissions: string[];
    databaseServer: ModCoreGuild;
};

export default function Manage({
    user,
    server,
    databaseServer,
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

    const originalWelcomeJson = JSON.parse(
        databaseServer.welcome_settings.welcome_message_json ?? "{}"
    ) as WelcomeMessageJson;

    // inertia UseForm:
    const { data, setData, post, processing, errors } = useForm({
        messagePayload: originalWelcomeJson,
        enabled: databaseServer.welcome_settings.enabled ?? false,
        channelId: databaseServer.welcome_settings.channel_id ?? 0,
    });

    // Function to update the messagePayload
    function setContent(content: string) {
        setData("messagePayload", {
            ...data.messagePayload,
            content,
        });
    }

    function setEnabled(enabled: boolean) {
        setData("enabled", enabled);
    }

    function setChannelId(channelId: string) {
        setData("channelId", BigInt(channelId));
    }

    return (
        <>
            <Head title="Welcome" />

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
                        <div className="grid md:grid-cols-2 gap-4">
                            <div>
                                <div className="bg-gray-800 p-4 rounded-lg shadow-md">
                                    <div className="mb-4">
                                        <label
                                            className="block text-white mb-2"
                                            htmlFor="content"
                                        >
                                            Message Content
                                        </label>
                                        <textarea
                                            id="content"
                                            className="w-full p-2 rounded bg-gray-700 text-white"
                                            value={data.messagePayload.content}
                                            onChange={(e) =>
                                                setContent(e.target.value)
                                            }
                                        />
                                    </div>
                                </div>
                                <div className="bg-gray-800 p-4 rounded-lg shadow-md mt-4">
                                    <h2 className="text-xl font-bold text-white mb-4">
                                        Embeds
                                    </h2>
                                    {data.messagePayload.embeds?.map(
                                        (embed, index) => (
                                            <div
                                                key={index}
                                                className="mb-6 p-4 bg-gray-700 rounded-lg shadow-md"
                                            >
                                                <h3 className="text-lg font-semibold text-white mb-4">
                                                    Embed {index + 1}
                                                </h3>
                                                <div className="mb-4">
                                                    <label
                                                        className="block text-white mb-2"
                                                        htmlFor={`embed-title-${index}`}
                                                    >
                                                        Title
                                                    </label>
                                                    <input
                                                        type="text"
                                                        id={`embed-title-${index}`}
                                                        className="w-full p-2 rounded bg-gray-600 text-white"
                                                        value={embed.title}
                                                        onChange={(e) => {
                                                            const newEmbeds = [
                                                                ...data
                                                                    .messagePayload
                                                                    .embeds,
                                                            ];
                                                            newEmbeds[
                                                                index
                                                            ].title =
                                                                e.target.value;
                                                            setData(
                                                                "messagePayload",
                                                                {
                                                                    ...data.messagePayload,
                                                                    embeds: newEmbeds,
                                                                }
                                                            );
                                                        }}
                                                    />
                                                </div>
                                                <div className="mb-4">
                                                    <label
                                                        className="block text-white mb-2"
                                                        htmlFor={`embed-description-${index}`}
                                                    >
                                                        Description
                                                    </label>
                                                    <textarea
                                                        id={`embed-description-${index}`}
                                                        className="w-full p-2 rounded bg-gray-600 text-white"
                                                        value={
                                                            embed.description
                                                        }
                                                        onChange={(e) => {
                                                            const newEmbeds = [
                                                                ...data
                                                                    .messagePayload
                                                                    .embeds,
                                                            ];
                                                            newEmbeds[
                                                                index
                                                            ].description =
                                                                e.target.value;
                                                            setData(
                                                                "messagePayload",
                                                                {
                                                                    ...data.messagePayload,
                                                                    embeds: newEmbeds,
                                                                }
                                                            );
                                                        }}
                                                    />
                                                </div>
                                                <div className="mb-4">
                                                    <label
                                                        className="block text-white mb-2"
                                                        htmlFor={`embed-url-${index}`}
                                                    >
                                                        URL
                                                    </label>
                                                    <input
                                                        type="text"
                                                        id={`embed-url-${index}`}
                                                        className="w-full p-2 rounded bg-gray-600 text-white"
                                                        value={embed.url}
                                                        onChange={(e) => {
                                                            const newEmbeds = [
                                                                ...data
                                                                    .messagePayload
                                                                    .embeds,
                                                            ];
                                                            newEmbeds[
                                                                index
                                                            ].url =
                                                                e.target.value;
                                                            setData(
                                                                "messagePayload",
                                                                {
                                                                    ...data.messagePayload,
                                                                    embeds: newEmbeds,
                                                                }
                                                            );
                                                        }}
                                                    />
                                                </div>
                                                <div className="mb-4">
                                                    <label
                                                        className="block text-white mb-2"
                                                        htmlFor={`embed-color-${index}`}
                                                    >
                                                        Color
                                                    </label>
                                                    <input
                                                        type="color"
                                                        id={`embed-color-${index}`}
                                                        className="w-full p-2 rounded bg-gray-600 text-white"
                                                        value={`#${embed.color?.toString(16).padStart(6, "0") ?? "000000"}`}
                                                        onChange={(e) => {
                                                            const newEmbeds = [
                                                                ...data.messagePayload.embeds,
                                                            ];
                                                            newEmbeds[index].color = parseInt(e.target.value.slice(1).toUpperCase(), 16);
                                                            setData("messagePayload", {
                                                                ...data.messagePayload,
                                                                embeds: newEmbeds,
                                                            });
                                                        }}
                                                    />
                                                </div>
                                                <div className="mb-4">
                                                    <h4 className="text-md font-semibold text-white mb-2">
                                                        Fields
                                                    </h4>
                                                    {embed.fields?.map(
                                                        (field, fieldIndex) => (
                                                            <div
                                                                key={fieldIndex}
                                                                className="mb-4 p-2 bg-gray-600 rounded-lg"
                                                            >
                                                                <div className="mb-2">
                                                                    <label
                                                                        className="block text-white mb-1"
                                                                        htmlFor={`embed-field-name-${index}-${fieldIndex}`}
                                                                    >
                                                                        Field
                                                                        Name
                                                                    </label>
                                                                    <input
                                                                        type="text"
                                                                        id={`embed-field-name-${index}-${fieldIndex}`}
                                                                        className="w-full p-2 rounded bg-gray-500 text-white"
                                                                        value={
                                                                            field.name
                                                                        }
                                                                        onChange={(
                                                                            e
                                                                        ) => {
                                                                            const newEmbeds =
                                                                                [
                                                                                    ...data
                                                                                        .messagePayload
                                                                                        .embeds,
                                                                                ];
                                                                            newEmbeds[
                                                                                index
                                                                            ].fields![
                                                                                fieldIndex
                                                                            ].name =
                                                                                e.target.value;
                                                                            setData(
                                                                                "messagePayload",
                                                                                {
                                                                                    ...data.messagePayload,
                                                                                    embeds: newEmbeds,
                                                                                }
                                                                            );
                                                                        }}
                                                                    />
                                                                </div>
                                                                <div className="mb-2">
                                                                    <label
                                                                        className="block text-white mb-1"
                                                                        htmlFor={`embed-field-value-${index}-${fieldIndex}`}
                                                                    >
                                                                        Field
                                                                        Value
                                                                    </label>
                                                                    <textarea
                                                                        id={`embed-field-value-${index}-${fieldIndex}`}
                                                                        className="w-full p-2 rounded bg-gray-500 text-white"
                                                                        value={
                                                                            field.value
                                                                        }
                                                                        onChange={(
                                                                            e
                                                                        ) => {
                                                                            const newEmbeds =
                                                                                [
                                                                                    ...data
                                                                                        .messagePayload
                                                                                        .embeds,
                                                                                ];
                                                                            newEmbeds[
                                                                                index
                                                                            ].fields![
                                                                                fieldIndex
                                                                            ].value =
                                                                                e.target.value;
                                                                            setData(
                                                                                "messagePayload",
                                                                                {
                                                                                    ...data.messagePayload,
                                                                                    embeds: newEmbeds,
                                                                                }
                                                                            );
                                                                        }}
                                                                    />
                                                                </div>
                                                                <div className="mb-2">
                                                                    <label
                                                                        className="block text-white mb-1"
                                                                        htmlFor={`embed-field-inline-${index}-${fieldIndex}`}
                                                                    >
                                                                        Inline
                                                                    </label>
                                                                    <input
                                                                        type="checkbox"
                                                                        id={`embed-field-inline-${index}-${fieldIndex}`}
                                                                        className="mr-2 leading-tight"
                                                                        checked={
                                                                            field.inline
                                                                        }
                                                                        onChange={(
                                                                            e
                                                                        ) => {
                                                                            const newEmbeds =
                                                                                [
                                                                                    ...data
                                                                                        .messagePayload
                                                                                        .embeds,
                                                                                ];
                                                                            newEmbeds[
                                                                                index
                                                                            ].fields![
                                                                                fieldIndex
                                                                            ].inline =
                                                                                e.target.checked;
                                                                            setData(
                                                                                "messagePayload",
                                                                                {
                                                                                    ...data.messagePayload,
                                                                                    embeds: newEmbeds,
                                                                                }
                                                                            );
                                                                        }}
                                                                    />
                                                                </div>
                                                                <button
                                                                    className="bg-red-500 hover:bg-red-700 text-white font-bold py-1 px-2 rounded mt-2"
                                                                    onClick={() => {
                                                                        const newEmbeds =
                                                                            [
                                                                                ...data
                                                                                    .messagePayload
                                                                                    .embeds,
                                                                            ];
                                                                        newEmbeds[
                                                                            index
                                                                        ].fields =
                                                                            newEmbeds[
                                                                                index
                                                                            ].fields!.filter(
                                                                                (
                                                                                    _,
                                                                                    i
                                                                                ) =>
                                                                                    i !==
                                                                                    fieldIndex
                                                                            );
                                                                        setData(
                                                                            "messagePayload",
                                                                            {
                                                                                ...data.messagePayload,
                                                                                embeds: newEmbeds,
                                                                            }
                                                                        );
                                                                    }}
                                                                >
                                                                    Remove Field
                                                                </button>
                                                            </div>
                                                        )
                                                    )}
                                                    <button
                                                        className="bg-green-500 hover:bg-green-700 text-white font-bold py-2 px-4 rounded mt-2"
                                                        onClick={() => {
                                                            const newEmbeds = [
                                                                ...data
                                                                    .messagePayload
                                                                    .embeds,
                                                            ];
                                                            newEmbeds[
                                                                index
                                                            ].fields = [
                                                                ...(newEmbeds[
                                                                    index
                                                                ].fields ?? []),
                                                                {
                                                                    name: "",
                                                                    value: "",
                                                                    inline: false,
                                                                },
                                                            ];
                                                            setData(
                                                                "messagePayload",
                                                                {
                                                                    ...data.messagePayload,
                                                                    embeds: newEmbeds,
                                                                }
                                                            );
                                                        }}
                                                    >
                                                        Add Field
                                                    </button>
                                                </div>
                                                <button
                                                    className="bg-red-500 hover:bg-red-700 text-white font-bold py-2 px-4 rounded"
                                                    onClick={() => {
                                                        const newEmbeds =
                                                            data.messagePayload.embeds.filter(
                                                                (_, i) =>
                                                                    i !== index
                                                            );
                                                        setData(
                                                            "messagePayload",
                                                            {
                                                                ...data.messagePayload,
                                                                embeds: newEmbeds,
                                                            }
                                                        );
                                                    }}
                                                >
                                                    Remove Embed
                                                </button>
                                            </div>
                                        )
                                    )}
                                    <button
                                        className="bg-green-500 hover:bg-green-700 text-white font-bold py-2 px-4 rounded"
                                        onClick={() => {
                                            const newEmbeds = [
                                                ...(data.messagePayload
                                                    .embeds || []),
                                                {},
                                            ];
                                            setData("messagePayload", {
                                                ...data.messagePayload,
                                                embeds: newEmbeds,
                                            });
                                        }}
                                    >
                                        Add Embed
                                    </button>
                                </div>
                            </div>
                            <FakeDiscordMessage
                                content={data.messagePayload.content}
                                embeds={
                                    data.messagePayload.embeds ?? undefined
                                }
                                className="h-fit sticky top-0"
                            />
                        </div>
                    </>
                )}
            </DashboardLayout>
        </>
    );
}
