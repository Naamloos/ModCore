import { DiscordGuild } from "@/Types/DiscordTypes/DiscordGuild";
import { User } from "@/Types/User";
import { usePage } from "@inertiajs/react";
import { IconCross, IconX } from "@tabler/icons-react";
import {
    Sheet,
    SheetContent,
    SheetDescription,
    SheetHeader,
    SheetTitle,
    SheetTrigger,
} from "@/Components/ui/sheet";
import { ScrollArea } from "@/Components/ui/scroll-area";
import { Button } from "@/Components/ui/button";
import { Avatar, AvatarFallback, AvatarImage } from "@/Components/ui/avatar";

export default function MobileServerMenu({
    isMenuOpen,
    setIsMenuOpen,
}: {
    isMenuOpen: boolean;
    setIsMenuOpen: (value: boolean) => void;
}) {
    const { user_guilds, server } = usePage<{
        user_guilds: DiscordGuild[];
        server: DiscordGuild | undefined;
    }>().props;

    return (
        <Sheet open={isMenuOpen} onOpenChange={setIsMenuOpen}>
            <SheetContent 
                side="left" 
                className="w-3/4 bg-gray-900 border-r border-gray-800 md:hidden"
                style={{ backdropFilter: 'blur(10px)' }}
            >
                <SheetHeader className="mb-4">
                    <SheetTitle>Server Menu</SheetTitle>
                    <SheetDescription>
                        Select a server to manage.
                    </SheetDescription>
                </SheetHeader>
                <ScrollArea className="h-[calc(100vh-100px)]">
                    <div className="flex flex-col space-y-2 px-2">
                        {user_guilds.map((discordGuild) => (
                            <Button
                                key={discordGuild.id}
                                variant="ghost"
                                className={`justify-start w-full hover:bg-gray-700 ${
                                    discordGuild.id === (server?.id ?? 0) ? "bg-gray-700" : ""
                                }`}
                                onClick={() => {
                                    window.location.href = `/dashboard/servers/${discordGuild.id}`;
                                    setIsMenuOpen(false); // Close the menu after selecting a server
                                }}
                            >
                                <Avatar className="mr-2 h-8 w-8">
                                    {discordGuild.icon ? (
                                        <AvatarImage src={`https://cdn.discordapp.com/icons/${discordGuild.id}/${discordGuild.icon}.png`} alt={discordGuild.name} />
                                    ) : (
                                        <AvatarFallback>
                                            {discordGuild.name
                                                .split(" ")
                                                .map((word) => word.charAt(0))
                                                .join("")}
                                        </AvatarFallback>
                                    )}
                                </Avatar>
                                <span>{discordGuild.name}</span>
                            </Button>
                        ))}
                    </div>
                </ScrollArea>
            </SheetContent>
        </Sheet>
    );
}
