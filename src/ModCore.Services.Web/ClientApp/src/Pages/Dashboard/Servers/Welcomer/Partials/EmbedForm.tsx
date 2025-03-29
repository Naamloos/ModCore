import { DiscordEmbed, DiscordEmbedField } from "@/Types/DiscordTypes/DiscordEmbed";
import { useState } from "react";
import EmbedFieldForm from "./EmbedFieldForm";
import { Input } from "@/Components/ui/input";
import { Textarea } from "@/Components/ui/textarea";
import { IconCross, IconCrossOff, IconX } from "@tabler/icons-react";
import ColorPicker from "@/Components/Forms/ColorPicker";
import { Switch } from "@/Components/ui/switch";
import { Button } from "@/Components/ui/button";

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
                className="relative mb-6 p-4 rounded-lg border-input border-[1px] shadow-md"
            >
                <div className="absolute top-2 right-2">
                    <Button
                        variant="destructive"
                        size="icon"
                        onClick={remove}
                    >
                        <IconX className="h-4 w-4" />
                    </Button>
                </div>
                <div className="mb-4">
                    <label className="block text-sm font-medium text-gray-200">
                        Title
                    </label>
                    <Input
                        value={embed.title || ""}
                        onChange={(e) => update({ ...embed, title: e.target.value })}
                        placeholder="Embed Title"
                    />
                </div>
                <div className="mb-4">
                    <label className="block text-sm font-medium text-gray-200">
                        Description
                    </label>
                    <Textarea
                        value={embed.description || ""}
                        onChange={(e) =>
                            update({ ...embed, description: e.target.value })
                        }
                        placeholder="Embed Description"
                        rows={4}
                    />
                </div>
                <div className="mb-4">
                    <label className="block text-sm font-medium text-gray-200">
                        URL
                    </label>
                    <Input
                        value={embed.url || ""}
                        onChange={(e) => update({ ...embed, url: e.target.value })}
                        placeholder="Embed URL"
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
                    <Switch
                        checked={!!embed.author}
                        onCheckedChange={(value) => {
                            let newEmbed = { ...embed };
                            if (value) {
                                newEmbed.author = { name: "", url: "", icon_url: "" };
                            } else {
                                delete newEmbed.author;
                            }
                            update(newEmbed);
                        }}
                    />
                    <label className="ml-2 text-sm font-medium text-gray-200">
                        Has Author?
                    </label>
                </div>
                {embed.author && <>
                    <div className="mb-4">
                        <label className="block text-sm font-medium text-gray-200">
                            Author Name
                        </label>
                        <Input
                            value={embed.author?.name || ""}
                            onChange={(e) => {
                                let newEmbed = { ...embed, author: { ...embed.author, name: e.target.value } };
                                update(newEmbed);
                            }}
                            placeholder="Author Name"
                        />
                    </div>
                    <div className="mb-4">
                        <label className="block text-sm font-medium text-gray-200">
                            Author URL
                        </label>
                        <Input
                            value={embed.author?.url || ""}
                            onChange={(e) => {
                                let newEmbed : DiscordEmbed = { ...embed };
                                newEmbed.author = { ...embed.author, name: embed.author?.name ?? "", url: e.target.value };
                                update(newEmbed);
                            }}
                            placeholder="Author URL"
                        />
                    </div>
                    <div className="mb-4">
                        <label className="block text-sm font-medium text-gray-200">
                            Author Icon URL
                        </label>
                        <Input
                            value={embed.author?.icon_url || ""}
                            onChange={(e) => {
                                let newEmbed = { ...embed, author: { ...embed.author, name: embed.author?.name ?? "", icon_url: e.target.value } };
                                update(newEmbed);
                            }}
                            placeholder="Author Icon URL"
                        />
                    </div>
                </>}

                <div className="mb-4">
                    <Switch
                        checked={!!embed.footer}
                        onCheckedChange={(value) => {
                            let newEmbed = { ...embed };
                            if (value) {
                                newEmbed.footer = { text: "", icon_url: "" };
                            } else {
                                delete newEmbed.footer;
                            }
                            update(newEmbed);
                        }}
                    />
                    <label className="ml-2 text-sm font-medium text-gray-200">
                        Has Footer?
                    </label>
                </div>

                {embed.footer && <>
                    <div className="mb-4">
                        <label className="block text-sm font-medium text-gray-200">
                            Footer Text
                        </label>
                        <Input
                            value={embed.footer?.text || ""}
                            onChange={(e) => {
                                let newEmbed = { ...embed, footer: { ...embed.footer, text: e.target.value } };
                                update(newEmbed);
                            }}
                            placeholder="Footer Text"
                        />
                    </div>
                    <div className="mb-4">
                        <label className="block text-sm font-medium text-gray-200">
                            Footer Icon URL
                        </label>
                        <Input
                            value={embed.footer?.icon_url || ""}
                            onChange={(e) => {
                                let newEmbed = { ...embed, footer: { ...embed.footer, text: embed.footer?.text ?? "", icon_url: e.target.value } };
                                update(newEmbed);
                            }}
                            placeholder="Footer Icon URL"
                        />
                    </div>
                </>}

                <div className="mb-4">
                    <Switch
                        checked={!!embed.image}
                        onCheckedChange={(value) => {
                            let newEmbed = { ...embed };
                            if (value) {
                                newEmbed.image = { url: "" };
                            } else {
                                delete newEmbed.image;
                            }
                            update(newEmbed);
                        }}
                    />
                    <label className="ml-2 text-sm font-medium text-gray-200">
                        Has Image?
                    </label>
                </div>

                {embed.image && <>
                    <div className="mb-4">
                        <label className="block text-sm font-medium text-gray-200">
                            Image URL
                        </label>
                        <Input
                            value={embed.image?.url || ""}
                            onChange={(e) => {
                                let newEmbed = { ...embed, image: { ...embed.image, url: e.target.value } };
                                update(newEmbed);
                            }}
                            placeholder="Image URL"
                        />
                    </div>
                </>}

                <div className="mb-4">
                    <Switch
                        checked={!!embed.thumbnail}
                        onCheckedChange={(value) => {
                            let newEmbed = { ...embed };
                            if (value) {
                                newEmbed.thumbnail = { url: "" };
                            } else {
                                delete newEmbed.thumbnail;
                            }
                            update(newEmbed);
                        }}
                    />
                    <label className="ml-2 text-sm font-medium text-gray-200">
                        Has Thumbnail?
                    </label>
                </div>

                {embed.thumbnail && <>
                    <div className="mb-4">
                        <label className="block text-sm font-medium text-gray-200">
                            Thumbnail URL
                        </label>
                        <Input
                            value={embed.thumbnail?.url || ""}
                            onChange={(e) => {
                                let newEmbed = { ...embed, thumbnail: { ...embed.thumbnail, url: e.target.value } };
                                update(newEmbed);
                            }}
                            placeholder="Thumbnail URL"
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
                    <Button
                        onClick={() => {
                            addField();
                        }}
                    >
                        Add Field
                    </Button>
                </div>
            </div>
        </>
    );
}
