import { DiscordChannel } from "@/Types/DiscordTypes/DiscordChannel";
import { DiscordRole } from "@/Types/DiscordTypes/DiscordRole";
import { usePage } from "@inertiajs/react";

interface DiscordChannelPickerProps {
    label: string;
    placeholder: string;
    value: string;
    onUpdate: (value: string) => void;
    required?: boolean;
}

export default function DiscordRolePicker({
    label,
    placeholder,
    value,
    onUpdate,
    required = false,
}: DiscordChannelPickerProps) {
    const { roles } = usePage<{ roles?: DiscordRole[] }>().props;

    if (!roles) {
        return (
            <>
                Channel Picker Component Error: prop "roles" was not present!
            </>
        );
    }

    return (
        <>
            <label className="block mb-2 text-sm font-medium text-white">
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
                    {roles.map((role) => (
                            <option key={role.id} value={role.id} selected={role.id === value}>
                                {role.name}
                            </option>
                        ))}
                </select>
            </div>
        </>
    );
}
