interface ConfirmPopupProps {
    message: string;
    content?: string;
    onConfirm: () => void;
    onCancel: () => void;
    confirmText?: string;
    cancelText?: string;
    visible?: boolean;
}

export default function ConfirmPopup({
    message,
    content = "",
    onConfirm,
    onCancel,
    confirmText = "Yes, I'm Sure",
    cancelText = "No, cancel",
    visible = false
}: ConfirmPopupProps) {
    return (
        visible && (
            <div
                id="popup-modal"
                tabIndex={-1}
                className="fixed inset-0 z-50 flex justify-center items-center"
                style={{ backgroundColor: "rgba(0, 0, 0, 0.5)", zIndex: 9999 }}
                onClick={onCancel}
            >
                <div
                    className="relative p-4 w-full max-w-md"
                    onClick={(e) => e.stopPropagation()}
                >
                    <div className="relative bg-gray-700 rounded-lg shadow">
                        <div className="p-4 md:p-5 text-center">
                            <svg
                                className="mx-auto mb-4 text-gray-200 w-12 h-12"
                                aria-hidden="true"
                                xmlns="http://www.w3.org/2000/svg"
                                fill="none"
                                viewBox="0 0 20 20"
                            >
                                <path
                                    stroke="currentColor"
                                    strokeLinecap="round"
                                    strokeLinejoin="round"
                                    strokeWidth="2"
                                    d="M10 11V6m0 8h.01M19 10a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z"
                                />
                            </svg>
                            <h3 className="text-lg font-normal text-gray-300">
                                {message}
                            </h3>
                            {content && (
                                <p className="text-gray-400 text-sm">{content}</p>
                            )}
                            <button
                                type="button"
                                className="mt-5 text-white bg-red-600 hover:bg-red-800 focus:ring-4 focus:outline-none focus:ring-red-800 font-medium rounded-lg text-sm inline-flex items-center px-5 py-2.5 text-center"
                                onClick={onConfirm}
                            >
                                {confirmText}
                            </button>
                            <button
                                type="button"
                                className="py-2.5 px-5 ml-3 text-sm font-medium text-gray-400 focus:outline-none bg-gray-800 rounded-lg border border-gray-600 hover:bg-gray-700 hover:text-white focus:z-10 focus:ring-4 focus:ring-gray-700"
                                onClick={onCancel}
                            >
                                {cancelText}
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        )
    );
}
