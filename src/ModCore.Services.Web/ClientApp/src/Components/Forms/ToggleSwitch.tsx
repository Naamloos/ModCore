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
        <>
            <label className="inline-flex items-center mb-5 cursor-pointer">
                <input
                    type="checkbox"
                    checked={enabled}
                    onChange={(e) => onUpdate(e.target.checked)}
                    className="sr-only peer"
                />
                <div className="relative w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-300 dark:peer-focus:ring-blue-800 rounded-full peer dark:bg-gray-600 peer-checked:after:translate-x-full rtl:peer-checked:after:-translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:start-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:w-5 after:h-5 after:transition-all dark:border-gray-600 peer-checked:bg-blue-600" style={{ zIndex: 1 }}></div>
                <span className="ms-3 text-sm font-medium text-gray-900 dark:text-gray-300">
                    {label}
                </span>
            </label>
        </>
    );
}
