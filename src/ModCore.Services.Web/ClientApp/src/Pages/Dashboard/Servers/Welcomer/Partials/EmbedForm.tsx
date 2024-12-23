import { DiscordEmbed, DiscordEmbedField } from "@/Types/DiscordTypes/DiscordEmbed";
import { useState } from "react";
import EmbedFieldForm from "./EmbedFieldForm";
import TextArea from "@/Components/Forms/TextArea";
import TextBox from "@/Components/Forms/TextBox";
import { IconCross, IconCrossOff, IconX } from "@tabler/icons-react";

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
                        value={`#${
                            embed.color?.toString(16).padStart(6, "0") ??
                            "000000"
                        }`}
                        onChange={(e) => {
                            let newEmbed = { ...embed, color: parseInt(e.target.value.slice(1).toUpperCase(), 16) };
                            update(newEmbed);
                        }}
                    />
                </div>
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
