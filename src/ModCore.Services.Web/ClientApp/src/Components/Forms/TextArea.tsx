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
        <>
            <label
                htmlFor="message"
                className="block mb-2 text-sm font-medium text-white"
            >
                {label}
            </label>
            <textarea
                id="message"
                rows={rows}
                className="block p-2.5 w-full text-sm text-white bg-gray-600 rounded-lg border border-gray-600 focus:ring-blue-500 focus:border-blue-500 placeholder-gray-400"
                placeholder={placeholder}
                value={value}
                onChange={(e) => onUpdate(e.target.value)}
                required={required}
            ></textarea>
        </>
    );
}
