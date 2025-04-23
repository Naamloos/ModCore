import { Input } from "@/Components/ui/input";

interface TextBoxProps {
  label: string;
  placeholder: string;
  value: string;
  onUpdate: (value: string) => void;
  required?: boolean;
}

export default function TextBox({
  label,
  placeholder,
  value,
  onUpdate,
  required = false,
}: TextBoxProps) {
  return (
    <div>
      <label className="block mb-2 text-sm font-medium text-white">
        {label}
      </label>
      <Input
        placeholder={placeholder}
        required={required}
        value={value}
        onChange={(e) => onUpdate(e.target.value)}
      />
    </div>
  );
}
