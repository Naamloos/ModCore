import { Textarea } from "@/Components/ui/textarea";

interface TextAreaProps {
  label: string;
  rows: number;
  placeholder: string;
  value: string;
  onUpdate: (value: string) => void;
  required?: boolean;
}

export default function TextArea({
  label,
  rows,
  placeholder,
  value,
  onUpdate,
  required = false,
}: TextAreaProps) {
  return (
    <div>
      <label className="block mb-2 text-sm font-medium text-white">
        {label}
      </label>
      <Textarea
        rows={rows}
        placeholder={placeholder}
        value={value}
        onChange={(e) => onUpdate(e.target.value)}
        required={required}
      />
    </div>
  );
}
