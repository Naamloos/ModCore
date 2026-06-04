import { useDiscordStore } from './store/useDiscordStore';
import { Spinner } from '@/components/ui/spinner';
import { useMemo, useState } from 'react';
import logo from '@/img/logo.png';
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';

function App() {
    const discord = useDiscordStore();
    const avatarUrl = useMemo(() => {
        return discord?.authResponse?.user?.avatar ? `https://cdn.discordapp.com/avatars/${discord.authResponse!.user.id}/${discord.authResponse!.user.avatar}.png` : logo;
    }, [discord?.authResponse])
    const [currentTab, setCurrentTab] = useState<string>('server');

    if (discord.isAuthenticated && discord.authResponse?.user.id !== '127408598010560513') {
        return <p>Hey you silly goose, I am still building this. If you're curious, ping @naamloos (Ryan)</p>
    }

    return (
        <>
            <div className="pt-[var(--sait)] pl-[var(--sail)] pr-[var(--sair)] pb-[var(--saib)]">
                <div className="w-full min-h-svh flex flex-col gap-4">
                    {discord.isMinimized ?
                        <>
                            <div className="self-center m-auto flex flex-col gap-2">
                                <img src={logo} className="h-18 w-18 m-auto" />
                                <h1 className="text-xl text-center">ModCore Configuration Utility</h1>
                            </div>
                        </> :
                        <>
                            {/* Loading state */}
                            {
                                !discord.isDoneLoading &&
                                <>

                                    <div className="self-center m-auto flex flex-col gap-4">
                                        <Spinner className="size-8 m-auto" />
                                        <h1 className="text-xl">Loading...</h1>
                                    </div>
                                </>
                            }
                            { /* Content when done loading */}
                            {
                                discord.isDoneLoading &&
                                <>
                                    { /* Top bar */}
                                    <div className="border-b-2 h-12 flex flex-row justify-between align-middle items-center px-4">
                                        <div className="flex flex-row gap-4 items-center">
                                            <img src={logo} className="h-8 w-8" />
                                            <span>ModCore Configuration Utility</span>
                                        </div>
                                        <div className="flex flex-row gap-4 items-center">
                                            <span>{discord.authResponse?.user?.global_name}</span>
                                            <img src={avatarUrl} className="h-8 w-8 rounded-full" />
                                        </div>
                                    </div>
                                    { /* Content */}
                                    <div className="flex flex-col gap-4 px-2">
                                        <Tabs defaultValue="server" onValueChange={value => setCurrentTab(value)} className="w-full">
                                            <TabsList variant='line'>
                                                <TabsTrigger value="server">Server Settings ({discord.currentGuild?.name})</TabsTrigger>
                                                <TabsTrigger value="profile">Personal Settings</TabsTrigger>
                                            </TabsList>
                                        </Tabs>
                                        <div className="px-1 flex flex-col gap-4">
                                            {
                                                /* big illegal move: inline switch in react */
                                                (() => {
                                                    switch (currentTab) {
                                                        default:
                                                            return <p>unknown tab</p>
                                                        case 'profile':
                                                            return <>
                                                                <img
                                                                    src={`https://cdn.discordapp.com/avatars/${discord.authResponse!.user.id}/${discord.authResponse!.user.avatar}.png`}
                                                                    alt={`${discord.authResponse!.user.username}'s avatar`}
                                                                    style={{ borderRadius: '50%', width: '48px', height: '48px' }}
                                                                />
                                                                <p>Welcome, {discord.authResponse!.user.global_name}</p>
                                                                <pre>
                                                                    {JSON.stringify(discord.authResponse!.user, null, 2)}
                                                                </pre>
                                                            </>
                                                        case 'server':
                                                            return <>
                                                                <p>Server Config</p>
                                                            </>
                                                    }
                                                })()
                                            }
                                        </div>
                                    </div>
                                </>
                            }
                        </>
                    }
                </div>
            </div>
        </>
    );
}

export default App;