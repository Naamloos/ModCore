import DiscordEmoji from "@/Types/DiscordTypes/DiscordEmoji";
import { usePage } from "@inertiajs/react";
import Picker from "@emoji-mart/react";
import data from "@emoji-mart/data";
import { useEffect, useState, useRef } from "react";

interface DiscordEmojiPickerProps {
    label: string;
    onUpdate: (value?: string) => void;
    value?: string;
}

export default function DiscordEmojiPicker({
    label,
    onUpdate,
    value,
}: DiscordEmojiPickerProps) {
    const { emojis } = usePage<{ emojis: DiscordEmoji[] }>().props;
    const [customEmojis, setCustomEmojis] = useState<any[]>([]);
    const [isOpen, setIsOpen] = useState(false);
    const pickerRef = useRef<HTMLDivElement>(null);

    const [pickedEmoji, setPickedEmoji] = useState<string | undefined>(value);
    const [isCustom, setIsCustom] = useState(false);

    useEffect(() => {
        if(value)
        {
            if(value.includes(":"))
            {
                const name = value.split(":")[0];
                const id = value.split(":")[1];
                const emoji = customEmojis[0]?.emojis.find((emoji: any) => emoji.id === id && emoji.name === name);
                if (emoji) {
                    setPickedEmoji(emoji.skins[0].src);
                    setIsCustom(true);
                }
            } else {
                setPickedEmoji(value);
                setIsCustom(false);
            }
        }else{
            setPickedEmoji(undefined);
            setIsCustom(false);
        }
    }, [value, customEmojis]);

    useEffect(() => {
        if (emojis) {
            const custom = [
                {
                    id: "custom",
                    name: "Custom",
                    emojis: emojis.map((emoji) => ({
                        id: emoji.id,
                        name: emoji.name || "custom_emoji",
                        keywords: [emoji.name || "custom_emoji"],
                        skins: [
                            {
                                src: `https://cdn.discordapp.com/emojis/${emoji.id}.${
                                    emoji.animated ? "gif" : "png"
                                }`,
                            },
                        ],
                    })),
                },
            ];
            setCustomEmojis(custom);
        }
    }, [emojis]);

    const handleEmojiSelect = (emoji: any) => {
        // if emoji is custom, return name:id. else, just return the unicode representation.
        if (!emoji.native) {
            onUpdate(`${emoji.name}:${emoji.id}`);
        } else {
            onUpdate(emoji.native);
        }
        setIsOpen(false);
    };

    const togglePicker = () => {
        setIsOpen(!isOpen);
    };

    useEffect(() => {
        function handleClickOutside(event: MouseEvent) {
            if (pickerRef.current && !pickerRef.current.contains(event.target as Node)) {
                setIsOpen(false);
            }
        }

        if (isOpen) {
            document.addEventListener("mousedown", handleClickOutside);
        } else {
            document.removeEventListener("mousedown", handleClickOutside);
        }

        return () => {
            document.removeEventListener("mousedown", handleClickOutside);
        };
    }, [isOpen]);

    return (
        <div className="relative">
            <button
                type="button"
                className="my-1 px-4 py-2 text-gray-200 bg-gray-700 rounded-md hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-opacity-50 dark:text-white"
                onClick={togglePicker}
            >
                {pickedEmoji ? <>
                    {isCustom ? <img src={pickedEmoji} alt="emoji" className="w-6 h-6 inline-block object-contain" /> : pickedEmoji}
                </> : "Select Emoji"}
            </button>

            {isOpen && (
                <div className="fixed top-0 left-0 w-full h-full flex items-center justify-center bg-gray-500 bg-opacity-50 z-50">
                    <div
                        ref={pickerRef}
                        className="bg-white dark:bg-gray-800 rounded-md shadow-lg"
                    >
                        {emojis ? (
                            <Picker
                                data={data}
                                onEmojiSelect={handleEmojiSelect}
                                custom={customEmojis}
                                width={400}
                                height={400}
                            />
                        ) : (
                            <div className="p-4">Emoji Picker Component Error: prop "emojis" was not present!</div>
                        )}
                    </div>
                </div>
            )}
        </div>
    );
}
