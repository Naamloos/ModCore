import TextArea from "@/Components/Forms/TextArea";
import TextBox from "@/Components/Forms/TextBox";
import ToggleSwitch from "@/Components/Forms/ToggleSwitch";
import { DiscordEmbedField } from "@/Types/DiscordTypes/DiscordEmbed";
import { useState } from "react";

interface EmbedFieldFormProps {
    embedIndex: number;
    fieldIndex: number;
    field: DiscordEmbedField;
    update: (field: DiscordEmbedField) => void;
    remove: () => void;
}

export default function EmbedFieldForm({
    embedIndex,
    fieldIndex,
    field,
    update,
    remove,
}: EmbedFieldFormProps) {
    return (
        <>
            <div key={fieldIndex} className="mb-4 p-2 border-gray-500 border-[1px] rounded-lg">
                <div className="mb-2">
                    <TextBox
                        label="Field Name"
                        placeholder="Field Name"
                        value={field.name}
                        onUpdate={(value) => {
                            let newField = { ...field, name: value };
                            update(newField);
                        }}
                    />
                </div>
                <div className="mb-2">
                    <TextArea
                        label="Field Value"
                        rows={3}
                        placeholder="Field Value"
                        value={field.value}
                        onUpdate={(value) => {
                            let newField = { ...field, value: value };
                            update(newField);
                        }}
                    />
                </div>
                <div className="mb-2">
                    <ToggleSwitch
                        enabled={field.inline ?? false}
                        onUpdate={(enabled) => {
                            let newField = {
                                ...field,
                                inline: enabled,
                            };
                            update(newField);
                        }}
                        label="Inline"
                    />
                </div>
                <button
                    className="bg-red-500 hover:bg-red-700 text-white font-bold py-1 px-2 rounded mt-2"
                    onClick={remove}
                >
                    Remove Field
                </button>
            </div>
        </>
    );
}
