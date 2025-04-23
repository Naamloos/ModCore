import { Input } from "@/Components/ui/input";
import { Textarea } from "@/Components/ui/textarea";
import { Switch } from "@/Components/ui/switch";
import { DiscordEmbedField } from "@/Types/DiscordTypes/DiscordEmbed";
import { Button } from "@/Components/ui/button";

interface EmbedFieldFormProps {
  fieldIndex: number;
  field: DiscordEmbedField;
  update: (field: DiscordEmbedField) => void;
  remove: () => void;
}

export default function EmbedFieldForm({
  fieldIndex,
  field,
  update,
  remove,
}: EmbedFieldFormProps) {
  return (
    <>
      <div
        key={fieldIndex}
        className="mb-4 p-2 border-input border-[1px] rounded-lg"
      >
        <div className="mb-2">
          <label className="block text-sm font-medium text-gray-200">
            Field Name
          </label>
          <Input
            placeholder="Field Name"
            value={field.name}
            onChange={(e) => {
              const newField = { ...field, name: e.target.value };
              update(newField);
            }}
          />
        </div>
        <div className="mb-2">
          <label className="block text-sm font-medium text-gray-200">
            Field Value
          </label>
          <Textarea
            rows={3}
            placeholder="Field Value"
            value={field.value}
            onChange={(e) => {
              const newField = { ...field, value: e.target.value };
              update(newField);
            }}
          />
        </div>
        <div className="mb-2 flex items-center">
          <Switch
            checked={field.inline ?? false}
            onCheckedChange={(enabled) => {
              const newField = {
                ...field,
                inline: enabled,
              };
              update(newField);
            }}
          />
          <label className="ml-2 text-sm font-medium text-gray-200">
            Inline
          </label>
        </div>
        <Button variant="destructive" onClick={remove}>
          Remove Field
        </Button>
      </div>
    </>
  );
}
