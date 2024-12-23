import { DiscordEmbed, DiscordEmbedField } from "@/Types/DiscordTypes/DiscordEmbed";
import { useState } from "react";
import EmbedFieldForm from "./EmbedFieldForm";
import TextArea from "@/Components/Forms/TextArea";
import TextBox from "@/Components/Forms/TextBox";
import { IconCross, IconCrossOff, IconX } from "@tabler/icons-react";
import ColorPicker from "@/Components/Forms/ColorPicker";
import ToggleSwitch from "@/Components/Forms/ToggleSwitch";

interface EmbedFormProps {
    index: number;
    embed: DiscordEmbed;
    update: (embed: DiscordEmbed) => void;
    remove: () => void;
}

export default function EmbedForm({
    index,
    embed,
    update,
    remove
}: EmbedFormProps) 
{
    function addField()
    {
        const newEmbed = { ...embed };
        newEmbed.fields = [...(newEmbed.fields ?? []), { name: "", value: "", inline: false }];
        update(newEmbed);
    }

    function updateField(field: DiscordEmbedField, fieldIndex: number)
    {
        const newEmbed = { ...embed };
        newEmbed.fields = [...(newEmbed.fields ?? [])];
        newEmbed.fields[fieldIndex] = field;
        update(newEmbed);
    }

    function removeField(fieldIndex: number)
    {
        const newEmbed = { ...embed };
        newEmbed.fields = [...(newEmbed.fields ?? [])];
        newEmbed.fields.splice(fieldIndex, 1);
        update(newEmbed);
    }

    return (
        <>
            <div
                className="relative mb-6 p-4 rounded-lg border-gray-500 border-[1px] shadow-md"
            >
                <div className="absolute top-2 right-2">
                    <button
                        className="bg-red-500 hover:bg-red-700 text-white font-bold rounded flex items-center justify-center h-6 w-6"
                        onClick={remove}
                    >
                        <IconX size={16} />
                    </button>
                </div>
                <div className="mb-4">
                    <TextBox
                        label="Title"
                        placeholder="Title"
                        value={embed.title ?? ""}
                        onUpdate={(value) => {
                            let newEmbed = { ...embed, title: value };
                            update(newEmbed);
                        }}
                    />
                </div>
                <div className="mb-4">
                    <TextArea
                        label="Description"
                        rows={4}
                        placeholder="Description"
                        value={embed.description ?? ""}
                        onUpdate={(value) => {
                            let newEmbed = { ...embed, description: value };
                            update(newEmbed);
                        }}
                    />
                </div>
                <div className="mb-4">
                    <TextBox
                        label="URL"
                        placeholder="URL"
                        value={embed.url ?? ""}
                        onUpdate={(value) => {
                            let newEmbed = { ...embed, url: value };
                            update(newEmbed);
                        }}
                    />
                </div>
                <div className="mb-4">
                    <ColorPicker
                        label="Color"
                        value={`#${
                            embed.color?.toString(16).padStart(6, "0") ??
                            "000000"
                        }`}
                        onUpdate={(value) => {
                            let newEmbed = { ...embed, color: parseInt(value.slice(1).toUpperCase(), 16) };
                            update(newEmbed);
                        }}
                    />
                </div>
                <div className="mb-4">
                    <ToggleSwitch
                        enabled={!!embed.author}
                        label="Has Author?"
                        onUpdate={(value) => {
                            let newEmbed = { ...embed };
                            if (value) {
                                newEmbed.author = { name: "", url: "", icon_url: "" };
                            } else {
                                delete newEmbed.author;
                            }
                            update(newEmbed);
                        }}
                    />
                </div>
                {embed.author && <>
                    <div className="mb-4">
                        <TextBox
                            label="Author Name"
                            placeholder="Author Name"
                            value={embed.author?.name ?? ""}
                            onUpdate={(value) => {
                                let newEmbed = { ...embed, author: { ...embed.author, name: value } };
                                update(newEmbed);
                            }}
                        />
                    </div>
                    <div className="mb-4">
                        <TextBox
                            label="Author URL"
                            placeholder="Author URL"
                            value={embed.author?.url ?? ""}
                            onUpdate={(value) => {
                                let newEmbed : DiscordEmbed = { ...embed };
                                newEmbed.author = { ...embed.author, name: embed.author?.name ?? "", url: value };
                                update(newEmbed);
                            }}
                        />
                    </div>
                    <div className="mb-4">
                        <TextBox
                            label="Author Icon URL"
                            placeholder="Author Icon URL"
                            value={embed.author?.icon_url ?? ""}
                            onUpdate={(value) => {
                                let newEmbed = { ...embed, author: { ...embed.author, name: embed.author?.name ?? "", icon_url: value } };
                                update(newEmbed);
                            }}
                        />
                    </div>
                </>}

                <div className="mb-4">
                    <ToggleSwitch
                        enabled={!!embed.footer}
                        label="Has Footer?"
                        onUpdate={(value) => {
                            let newEmbed = { ...embed };
                            if (value) {
                                newEmbed.footer = { text: "", icon_url: "" };
                            } else {
                                delete newEmbed.footer;
                            }
                            update(newEmbed);
                        }}
                    />
                </div>

                {embed.footer && <>
                    <div className="mb-4">
                        <TextBox
                            label="Footer Text"
                            placeholder="Footer Text"
                            value={embed.footer?.text ?? ""}
                            onUpdate={(value) => {
                                let newEmbed = { ...embed, footer: { ...embed.footer, text: value } };
                                update(newEmbed);
                            }}
                        />
                    </div>
                    <div className="mb-4">
                        <TextBox
                            label="Footer Icon URL"
                            placeholder="Footer Icon URL"
                            value={embed.footer?.icon_url ?? ""}
                            onUpdate={(value) => {
                                let newEmbed = { ...embed, footer: { ...embed.footer, text: embed.footer?.text ?? "", icon_url: value } };
                                update(newEmbed);
                            }}
                        />
                    </div>
                </>}

                <div className="mb-4">
                    <ToggleSwitch
                        enabled={!!embed.image}
                        label="Has Image?"
                        onUpdate={(value) => {
                            let newEmbed = { ...embed };
                            if (value) {
                                newEmbed.image = { url: "" };
                            } else {
                                delete newEmbed.image;
                            }
                            update(newEmbed);
                        }}
                    />
                </div>

                {embed.image && <>
                    <div className="mb-4">
                        <TextBox
                            label="Image URL"
                            placeholder="Image URL"
                            value={embed.image?.url ?? ""}
                            onUpdate={(value) => {
                                let newEmbed = { ...embed, image: { ...embed.image, url: value } };
                                update(newEmbed);
                            }}
                        />
                    </div>
                </>}

                <div className="mb-4">
                    <ToggleSwitch
                        enabled={!!embed.thumbnail}
                        label="Has Thumbnail?"
                        onUpdate={(value) => {
                            let newEmbed = { ...embed };
                            if (value) {
                                newEmbed.thumbnail = { url: "" };
                            } else {
                                delete newEmbed.thumbnail;
                            }
                            update(newEmbed);
                        }}
                    />
                </div>

                {embed.thumbnail && <>
                    <div className="mb-4">
                        <TextBox
                            label="Thumbnail URL"
                            placeholder="Thumbnail URL"
                            value={embed.thumbnail?.url ?? ""}
                            onUpdate={(value) => {
                                let newEmbed = { ...embed, thumbnail: { ...embed.thumbnail, url: value } };
                                update(newEmbed);
                            }}
                        />
                    </div>
                </>}

                <div className="mb-4">
                    <h4 className="text-md font-semibold text-white mb-2">
                        Fields
                    </h4>
                    {embed.fields?.map((field, fieldIndex) => 
                        <EmbedFieldForm
                            embedIndex={index}
                            fieldIndex={fieldIndex}
                            field={field}
                            update={(newField) => updateField(newField, fieldIndex)}
                            remove={() => removeField(fieldIndex)}
                        />
                    )}
                    <button
                        className="bg-green-500 hover:bg-green-700 text-white font-bold py-2 px-4 rounded mt-2"
                        onClick={() => {
                            addField();
                        }}
                    >
                        Add Field
                    </button>
                </div>
            </div>
        </>
    );
}
