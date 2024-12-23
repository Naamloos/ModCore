import { useState } from 'react';

interface ColorPickerProps {
    label: string;
    value: string;
    onUpdate: (value: string) => void;
}

export default function ColorPicker({ label, value, onUpdate }: ColorPickerProps) {
    const [color, setColor] = useState(value);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setColor(e.target.value);
        onUpdate(e.target.value);
    };

    return (
        <div className="mb-5">
            <label className="block mb-2 text-sm font-medium text-white">{label}</label>
            <div className="flex items-center">
                <input
                    type="color"
                    value={color}
                    onChange={handleChange}
                    className="w-10 h-10 p-0 border-none cursor-pointer rounded-md"
                />
                <input
                    type="text"
                    value={color}
                    onChange={handleChange}
                    className="ml-2 p-2.5 bg-gray-600 border border-gray-600 text-white text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500"
                />
            </div>
        </div>
    );
}