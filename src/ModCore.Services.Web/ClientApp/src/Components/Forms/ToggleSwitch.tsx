import { Switch } from "@/Components/ui/switch";

interface ToggleSwitchProps {
    enabled: boolean;
    onUpdate: (enabled: boolean) => void;
    label: string;
}

export default function ToggleSwitch({
    enabled,
    onUpdate,
    label,
}: ToggleSwitchProps) {
    return (
        <div className="flex items-center">
            <Switch
                checked={enabled}
                onCheckedChange={onUpdate}
            />
            <label className="ml-2 text-sm font-medium text-gray-300">
                {label}
            </label>
        </div>
    );
}
