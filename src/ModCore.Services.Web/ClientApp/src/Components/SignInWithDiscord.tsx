import DiscordIcon from "@/Resources/discord-mark-white.svg?react";

interface SignInWithDiscordProps {
    onClick: () => void;
}

export default function SignInWithDiscord({ onClick }: SignInWithDiscordProps) {
    return (
        <>
            <button
                type="button"
                className="text-white bg-[#5865F2] hover:bg-[#5865F2]/90 focus:ring-4 focus:outline-none font-semibold rounded-lg text-sm px-5 py-2.5 text-center inline-flex items-center focus:ring-[#5865F2]/55 me-2 mb-2 dark"
                onClick={onClick}
            >
                <DiscordIcon className="w-5 h-5" />
                <span className="pl-2">Sign in with Discord</span>
            </button>
        </>
    );
}
