import { DiscordChannel } from "@/Types/DiscordTypes/DiscordChannel";
import { usePage } from "@inertiajs/react";
import {
    Select,
    SelectTrigger,
    SelectValue,
    SelectContent,
    SelectItem,
} from "@/Components/ui/select";

interface DiscordChannelPickerProps {
    label: string;
    placeholder: string;
    value?: string;
    onUpdate: (value?: string) => void;
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

    return (
        <>
            <label className="block mb-2 text-sm font-medium text-gray-200 dark:text-white">
                {label}
            </label>
            <Select onValueChange={(val) => onUpdate(val == "#"? undefined : val)} defaultValue={value} value={value}>
                <SelectTrigger className="w-full">
                    <SelectValue placeholder={placeholder} />
                </SelectTrigger>
                <SelectContent>
                    {!required && (
                        <SelectItem key="none" value="#">
                            None
                        </SelectItem>
                    )}
                    {channels
                        .filter((channel) => channel.type === 0)
                        .map((channel) => (
                            <SelectItem key={channel.id} value={channel.id}>
                                #{channel.name}
                            </SelectItem>
                        ))}
                </SelectContent>
            </Select>
        </>
    );
}
