import { DiscordApplication } from "@/Types/DiscordTypes/DiscordApplication";
import { DiscordEmbed } from "@/Types/DiscordTypes/DiscordEmbed";
import { usePage } from "@inertiajs/react";

export default function FakeDiscordMessage({
    content,
    embeds,
    className,
}: {
    content: string;
    embeds?: DiscordEmbed[];
    className?: string;
}) {
    const { application } = usePage<{ application: DiscordApplication }>()
        .props;

    return (
        <div
            className={`flex items-start mb-4 bg-[#2f3136] rounded-lg shadow-lg px-6 py-8 max-w-full ${
                className ?? ""
            }`}
        >
            <img
                src={`https://cdn.discordapp.com/app-icons/${application.id}/${application.icon}.png`}
                className="h-12 w-12 rounded-full"
                alt="User Avatar"
            />
            <div className="ml-4 flex-1">
                <div className="flex items-center">
                    <h2 className="text-base font-medium text-green-600">
                        {application.name}
                        <span className="inline-block font-semibold text-sm text-white px-1 rounded-md ml-2 bg-[#5865f2]">
                            APP
                        </span>
                    </h2>
                    <span className="text-xs text-gray-400 ml-2">Just Now</span>
                </div>
                <p className="text-white mt-1">{content}</p>
                {/* fake Discord embeds */}
                {embeds && embeds.map((embed, index) => {
                    const color = embed?.color
                        ? embed.color.toString(16).padStart(6, "0")
                        : "1E1F22";
                    return (
                        <div
                            key={index}
                            className={`mt-4 p-4 bg-[#36393f] rounded-lg border-l-[0.5rem] z-0 max-w-full sm:max-w-[650px]`}
                            style={{ borderLeftColor: `#${color}` }}
                        >
                            {embed.author && (
                                <div className="flex items-center mb-2">
                                    {embed.author.icon_url && (
                                        <img
                                            src={embed.author.icon_url}
                                            className="h-6 w-6 rounded-full mr-2"
                                            alt="Author Icon"
                                        />
                                    )}
                                    <span className="text-sm font-semibold text-white">
                                        {embed.author.name}
                                    </span>
                                </div>
                            )}
                            <div className={`grid ${embed.thumbnail?.url ? 'grid-cols-[auto_80px]' : ''} gap-4`}>
                                <div>
                                    {embed.title && (
                                        <h3 className="text-lg font-semibold text-white">
                                            {embed.title}
                                        </h3>
                                    )}
                                    {embed.description && (
                                        <p className="text-sm text-gray-300 mt-2">
                                            {embed.description}
                                        </p>
                                    )}
                                    {embed.fields && embed.fields.length > 0 && (
                                        <div className="grid grid-cols-2 gap-4 mt-4">
                                            {embed.fields.map((field, index) => (
                                                <div
                                                    key={index}
                                                    className={`mt-2 align-top ${
                                                        field.inline
                                                            ? "col-span-1"
                                                            : "col-span-2"
                                                    }`}
                                                >
                                                    <p className="text-sm font-semibold text-white">
                                                        {field.name}
                                                    </p>
                                                    <p className="text-sm text-gray-300">
                                                        {field.value}
                                                    </p>
                                                </div>
                                            ))}
                                        </div>
                                    )}
                                    <div className="flex mt-4">
                                        <div className="flex-1">
                                            {embed.image && (
                                                <img
                                                    src={embed.image.url}
                                                    className="max-w-full h-auto rounded-lg"
                                                    alt="Embed Image"
                                                />
                                            )}
                                        </div>
                                    </div>
                                </div>
                                {embed.thumbnail && (
                                    <img src={embed.thumbnail.url} className="h-auto rounded-lg" alt="Thumbnail" />
                                )}
                            </div>
                            {embed.footer && (
                                <div className="mt-4 flex items-center">
                                    {embed.footer.icon_url && (
                                        <img
                                            src={embed.footer.icon_url}
                                            className="h-4 w-4 mr-2"
                                            alt="Footer Icon"
                                        />
                                    )}
                                    <span className="text-xs text-gray-400">
                                        {embed.footer.text}
                                    </span>
                                </div>
                            )}
                        </div>
                    );
                })}
            </div>
        </div>
    );
}
