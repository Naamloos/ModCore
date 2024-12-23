import { DiscordChannel } from "@/Types/DiscordTypes/DiscordChannel";
import { usePage } from "@inertiajs/react";

interface DiscordChannelPickerProps {
    label: string;
    placeholder: string;
    value: string;
    onUpdate: (value: string) => void;
    required?: boolean;
}

export default function DiscordChannelPicker({
    label,
    placeholder,
    value,
    onUpdate,
    required = false,
}: DiscordChannelPickerProps) {
    const { channels } = usePage<{ channels?: DiscordChannel[] }>().props;

    if (!channels) {
        return (
            <>
                Channel Picker Component Error: prop "channels" was not present!
            </>
        );
    }

    console.log(value);

    return (
        <>
            <label className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                {label}
            </label>
            <div className="relative">
                <select
                    className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                    required={required}
                    value={value}
                    onChange={(e) => onUpdate(e.target.value)}
                >
                    <option value="">{placeholder}</option>
                    {channels
                        .filter((channel) => channel.type === 0)
                        .map((channel) => (
                            <option key={channel.id} value={channel.id} selected={channel.id === value}>
                                #{channel.name}
                            </option>
                        ))}
                </select>
            </div>
        </>
    );
}
