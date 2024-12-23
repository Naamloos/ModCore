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
        <>
            <label
                htmlFor="email"
                className="block mb-2 text-sm font-medium text-white"
            >
                {label}
            </label>
            <input
                className="shadow-sm bg-gray-600 border border-gray-600 placeholder-gray-400 text-white text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 shadow-sm-light"
                placeholder={placeholder}
                required={required}
                value={value}
                onChange={(e) => onUpdate(e.target.value)}
            />
        </>
    );
}
