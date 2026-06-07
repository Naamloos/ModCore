import { useMemo, useState } from 'react';
import { useDiscordStore } from './store/useDiscordStore';

import logo from '@/img/logo.png';

import { Spinner } from '@/components/ui/spinner';
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Separator } from '@/components/ui/separator';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from '@/components/ui/select';

type MainTab = 'server' | 'profile';

type ServerSettingsCategory =
    | 'autorole'
    | 'ban-appeals'
    | 'infractions'
    | 'jump-link-embeds'
    | 'levels'
    | 'logging'
    | 'profile-states'
    | 'role-menus'
    | 'starboards'
    | 'tags'
    | 'tickets'
    | 'welcomer';

type UserSettingsCategory =
    | 'overview'
    | 'account'
    | 'notifications'
    | 'privacy'
    | 'appearance';

const serverCategories: Array<{
    value: ServerSettingsCategory;
    label: string;
}> = [
        {
            value: 'autorole',
            label: 'AutoRole',
        },
        {
            value: 'ban-appeals',
            label: 'Ban Appeals',
        },
        {
            value: 'infractions',
            label: 'Infractions',
        },
        {
            value: 'jump-link-embeds',
            label: 'Jump Link Embeds',
        },
        {
            value: 'levels',
            label: 'Levels',
        },
        {
            value: 'logging',
            label: 'Logging',
        },
        {
            value: 'profile-states',
            label: 'Profile States',
        },
        {
            value: 'role-menus',
            label: 'Role Menus',
        },
        {
            value: 'starboards',
            label: 'Starboards',
        },
        {
            value: 'tags',
            label: 'Tags',
        },
        {
            value: 'tickets',
            label: 'Tickets',
        },
        {
            value: 'welcomer',
            label: 'Welcomer',
        },
    ];

const userCategories: Array<{
    value: UserSettingsCategory;
    label: string;
}> = [
        {
            value: 'overview',
            label: 'Overview',
        },
        {
            value: 'notifications',
            label: 'Notifications',
        },
        {
            value: 'privacy',
            label: 'Privacy',
        },
    ];

function App() {
    const discord = useDiscordStore();

    const avatarUrl = useMemo(() => {
        return discord?.authResponse?.user?.avatar
            ? `https://cdn.discordapp.com/avatars/${discord.authResponse.user.id}/${discord.authResponse.user.avatar}.png`
            : logo;
    }, [discord?.authResponse]);

    const [currentTab, setCurrentTab] = useState<MainTab>('server');
    const [serverCategory, setServerCategory] = useState<ServerSettingsCategory>('autorole');
    const [userCategory, setUserCategory] = useState<UserSettingsCategory>('overview');

    const selectedServerCategory = serverCategories.find(category => category.value === serverCategory);
    const selectedUserCategory = userCategories.find(category => category.value === userCategory);

    if (discord.isAuthenticated && discord.authResponse?.user.id !== '127408598010560513') {
        return <p>Hey you silly goose, I am still building this. If you're curious, ping @naamloos (Ryan)</p>;
    }

    return (
        <div className="min-h-svh pt-[var(--sait)] pl-[var(--sail)] pr-[var(--sair)] pb-[var(--saib)] text-foreground bg-black">
            <div className="flex min-h-svh w-full flex-col">
                {discord.isMinimized ? (
                    <div className="m-auto flex flex-col gap-2 self-center">
                        <img src={logo} className="m-auto h-18 w-18" alt="ModCore logo" />
                        <h1 className="text-center text-xl font-semibold">ModCore Configuration Utility</h1>
                    </div>
                ) : (
                    <>
                        {!discord.isDoneLoading && (
                            <div className="m-auto flex flex-col gap-4 self-center">
                                <Spinner className="m-auto size-8" />
                                <h1 className="text-xl font-semibold">Loading...</h1>
                            </div>
                        )}

                        {discord.isDoneLoading && (
                            <>
                                <header className="sticky top-0 z-20 border-b backdrop-blur">
                                    <div className="flex min-h-16 min-w-0 flex-col gap-3 px-4 py-2 md:flex-row md:items-center md:justify-between">
                                        <div className="flex min-w-0 shrink-0 items-center gap-3">
                                            <img src={logo} className="h-8 w-8 shrink-0" alt="ModCore logo" />

                                            <div className="min-w-0">
                                                <p className="text-sm font-medium text-muted-foreground">
                                                    ModCore
                                                </p>
                                                <h1 className="text-lg font-semibold">
                                                    Configuration Utility
                                                </h1>
                                            </div>
                                        </div>

                                        <div className="flex min-w-0 flex-col gap-3 sm:flex-row sm:items-center sm:justify-between md:flex-1 md:justify-end">
                                            <div className="min-w-0 overflow-x-auto overflow-y-hidden">
                                                <Tabs
                                                    value={currentTab}
                                                    onValueChange={value => setCurrentTab(value as MainTab)}
                                                    className="w-max min-w-full"
                                                >
                                                    <TabsList
                                                        variant="line"
                                                        className="h-auto w-max min-w-full flex-nowrap justify-start overflow-y-hidden"
                                                    >
                                                        <TabsTrigger
                                                            value="server"
                                                            className="shrink-0 whitespace-nowrap"
                                                        >
                                                            {discord.currentGuild?.name ?? 'Server'}
                                                        </TabsTrigger>

                                                        <TabsTrigger
                                                            value="profile"
                                                            className="shrink-0 whitespace-nowrap"
                                                        >
                                                            {discord.authResponse.user?.global_name ?? 'User'}
                                                        </TabsTrigger>
                                                    </TabsList>
                                                </Tabs>
                                            </div>

                                            <Avatar className="h-9 w-9 shrink-0 hidden md:block">
                                                <AvatarImage
                                                    src={avatarUrl}
                                                    alt={`${discord.authResponse.user?.username ?? 'User'} avatar`}
                                                />
                                                <AvatarFallback>
                                                    {discord.authResponse.user?.global_name?.charAt(0) ??
                                                        discord.authResponse.user?.username?.charAt(0) ??
                                                        'U'}
                                                </AvatarFallback>
                                            </Avatar>
                                        </div>
                                    </div>
                                </header>

                                <main className="flex flex-1 flex-col gap-4 p-2 lg:p-5">
                                    {currentTab === 'server' && (
                                        <section className="grid flex-1 gap-4 lg:grid-cols-[20rem_1fr]">
                                            <div className="lg:hidden">
                                                <MobileSectionSelect
                                                    label="Server section"
                                                    value={serverCategory}
                                                    categories={serverCategories}
                                                    onValueChange={value => setServerCategory(value as ServerSettingsCategory)}
                                                />
                                            </div>

                                            <Card className="hidden h-fit lg:block">
                                                <CardHeader>
                                                    <div className="flex items-start justify-between pb-3">
                                                        <div>
                                                            <CardTitle>Server Settings</CardTitle>
                                                        </div>

                                                        <Badge variant="secondary">Server</Badge>
                                                    </div>
                                                </CardHeader>

                                                <CardContent className="flex flex-col gap-2">
                                                    {serverCategories.map(category => (
                                                        <Button
                                                            key={category.value}
                                                            type="button"
                                                            variant={
                                                                serverCategory === category.value
                                                                    ? 'secondary'
                                                                    : 'ghost'
                                                            }
                                                            className="h-auto justify-start px-3 py-3 text-left"
                                                            onClick={() => setServerCategory(category.value)}
                                                        >
                                                            <span className="flex min-w-0 flex-col gap-1">
                                                                <span className="font-medium">
                                                                    {category.label}
                                                                </span>
                                                            </span>
                                                        </Button>
                                                    ))}
                                                </CardContent>
                                            </Card>

                                            <Card className="min-w-0">
                                                <CardHeader>
                                                    <div className="flex flex-col gap-2 sm:flex-row sm:items-start sm:justify-between">
                                                        <div>
                                                            <CardTitle>
                                                                {selectedServerCategory?.label ?? 'Unknown category'}
                                                            </CardTitle>
                                                        </div>

                                                        <Badge variant="outline" className="w-fit">
                                                            {discord.currentGuild?.name ?? 'Current server'}
                                                        </Badge>
                                                    </div>
                                                </CardHeader>

                                                <CardContent className="space-y-6">
                                                    {serverCategory === 'autorole' && (
                                                        <SettingsPlaceholder
                                                            title="AutoRole"
                                                            description="Configure which roles should automatically be granted to members when they newly join the server."
                                                        />
                                                    )}

                                                    {serverCategory === 'ban-appeals' && (
                                                        <SettingsPlaceholder
                                                            title="Ban Appeals"
                                                            description="Manage ban appeal intake, review status, moderator decisions, and appeal response settings."
                                                        />
                                                    )}

                                                    {serverCategory === 'infractions' && (
                                                        <SettingsPlaceholder
                                                            title="Infractions"
                                                            description="Manage member infractions, moderation history, warning records, active punishments, and audit trails."
                                                        />
                                                    )}

                                                    {serverCategory === 'jump-link-embeds' && (
                                                        <SettingsPlaceholder
                                                            title="Jump Link Embeds"
                                                            description="Configure automatic embeds when users post jump links in chat."
                                                        />
                                                    )}

                                                    {serverCategory === 'levels' && (
                                                        <SettingsPlaceholder
                                                            title="Levels"
                                                            description="Configure XP gain from chatting, leaderboard behavior, ignored channels, role rewards, and optional level-up messages."
                                                        />
                                                    )}

                                                    {serverCategory === 'logging' && (
                                                        <SettingsPlaceholder
                                                            title="Logging"
                                                            description="Configure logging for message deletions and edits, member joins, member updates, server changes, moderation actions, and more."
                                                        />
                                                    )}

                                                    {serverCategory === 'profile-states' && (
                                                        <SettingsPlaceholder
                                                            title="Profile States"
                                                            description="Configure whether old members that rejoin receive their previous roles and/or nicknames back."
                                                        />
                                                    )}

                                                    {serverCategory === 'role-menus' && (
                                                        <SettingsPlaceholder
                                                            title="Role Menus"
                                                            description="Create and manage in-chat role menus for self-assignable roles."
                                                        />
                                                    )}

                                                    {serverCategory === 'starboards' && (
                                                        <SettingsPlaceholder
                                                            title="Starboards"
                                                            description="Configure starboard channels, emoji triggers, ignored channels, and more."
                                                        />
                                                    )}

                                                    {serverCategory === 'tags' && (
                                                        <SettingsPlaceholder
                                                            title="Tags"
                                                            description="Manage short stored texts that members can retrieve at any time."
                                                        />
                                                    )}

                                                    {serverCategory === 'tickets' && (
                                                        <SettingsPlaceholder
                                                            title="Tickets"
                                                            description="Configure moderator support tickets, ticket categories, permissions, transcripts, and closing behavior."
                                                        />
                                                    )}

                                                    {serverCategory === 'welcomer' && (
                                                        <SettingsPlaceholder
                                                            title="Welcomer"
                                                            description="Configure custom welcome messages in chat for new members joining the server."
                                                        />
                                                    )}
                                                </CardContent>
                                            </Card>
                                        </section>
                                    )}

                                    {currentTab === 'profile' && (
                                        <section className="grid flex-1 gap-4 lg:grid-cols-[20rem_1fr]">
                                            <div className="lg:hidden">
                                                <MobileSectionSelect
                                                    label="User section"
                                                    value={userCategory}
                                                    categories={userCategories}
                                                    onValueChange={value => setUserCategory(value as UserSettingsCategory)}
                                                />
                                            </div>

                                            <Card className="hidden h-fit lg:block">
                                                <CardHeader>
                                                    <div className="flex items-start justify-between pb-3">
                                                        <div>
                                                            <CardTitle>User Settings</CardTitle>
                                                        </div>
                                                    </div>
                                                </CardHeader>

                                                <CardContent className="flex flex-col gap-2">
                                                    {userCategories.map(category => (
                                                        <Button
                                                            key={category.value}
                                                            type="button"
                                                            variant={
                                                                userCategory === category.value
                                                                    ? 'secondary'
                                                                    : 'ghost'
                                                            }
                                                            className="h-auto justify-start px-3 py-3 text-left"
                                                            onClick={() => setUserCategory(category.value)}
                                                        >
                                                            <span className="flex min-w-0 flex-col gap-1">
                                                                <span className="font-medium">
                                                                    {category.label}
                                                                </span>
                                                            </span>
                                                        </Button>
                                                    ))}
                                                </CardContent>
                                            </Card>

                                            <Card className="min-w-0">
                                                <CardHeader>
                                                    <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
                                                        <div className="flex min-w-0 items-center gap-3">
                                                            <Avatar className="h-12 w-12 shrink-0">
                                                                <AvatarImage
                                                                    src={avatarUrl}
                                                                    alt={`${discord.authResponse.user?.username ?? 'User'} avatar`}
                                                                />
                                                                <AvatarFallback>
                                                                    {discord.authResponse.user?.global_name?.charAt(0) ??
                                                                        discord.authResponse.user?.username?.charAt(0) ??
                                                                        'U'}
                                                                </AvatarFallback>
                                                            </Avatar>

                                                            <div className="min-w-0">
                                                                <CardTitle>
                                                                    {selectedUserCategory?.label ?? 'Unknown category'}
                                                                </CardTitle>
                                                            </div>
                                                        </div>

                                                        <Badge variant="outline" className="w-fit">
                                                            {discord.authResponse.user?.global_name ??
                                                                discord.authResponse.user?.username ??
                                                                'Current user'}
                                                        </Badge>
                                                    </div>
                                                </CardHeader>

                                                <CardContent className="space-y-6">
                                                    {userCategory === 'overview' && (
                                                        <>
                                                            <SettingsPlaceholder
                                                                title="Profile"
                                                                description="Your ModCore profile."
                                                            />

                                                            <Separator />

                                                            <div className="space-y-2">
                                                                <h3 className="text-sm font-medium">
                                                                    Discord user payload
                                                                </h3>

                                                                <pre className="max-h-96 overflow-auto rounded-lg border bg-muted p-4 text-xs text-muted-foreground">
                                                                    {JSON.stringify(discord.authResponse.user, null, 2)}
                                                                </pre>
                                                            </div>
                                                        </>
                                                    )}

                                                    {userCategory === 'notifications' && (
                                                        <SettingsPlaceholder
                                                            title="Notification settings"
                                                            description="How and when may ModCore reach out to you?"
                                                        />
                                                    )}

                                                    {userCategory === 'privacy' && (
                                                        <SettingsPlaceholder
                                                            title="Privacy settings"
                                                            description="Privacy settings and controls for ModCore's functionality in servers it shares with you."
                                                        />
                                                    )}
                                                </CardContent>
                                            </Card>
                                        </section>
                                    )}

                                    {currentTab !== 'server' && currentTab !== 'profile' && (
                                        <Card>
                                            <CardHeader>
                                                <CardTitle>Unknown tab</CardTitle>
                                                <CardDescription>
                                                    The selected dashboard tab does not exist.
                                                </CardDescription>
                                            </CardHeader>
                                        </Card>
                                    )}
                                </main>
                            </>
                        )}
                    </>
                )}
            </div>
        </div>
    );
}

function MobileSectionSelect<TValue extends string>(props: {
    label: string;
    value: TValue;
    categories: Array<{
        value: TValue;
        label: string;
    }>;
    onValueChange: (value: TValue) => void;
}) {
    return (
        <Card>
            <CardHeader className="pb-3">
                <CardTitle className="text-base">{props.label}</CardTitle>
                <CardDescription>
                    Choose which settings section to configure.
                </CardDescription>
            </CardHeader>

            <CardContent>
                <Select value={props.value} onValueChange={value => props.onValueChange(value as TValue)}>
                    <SelectTrigger className="w-full">
                        <SelectValue placeholder="Select a section" />
                    </SelectTrigger>

                    <SelectContent>
                        {props.categories.map(category => (
                            <SelectItem key={category.value} value={category.value}>
                                {category.label}
                            </SelectItem>
                        ))}
                    </SelectContent>
                </Select>
            </CardContent>
        </Card>
    );
}

function SettingsPlaceholder(props: {
    title: string;
    description: string;
}) {
    return (
        <div className="rounded-lg border bg-card p-4">
            <div className="space-y-2">
                <h2 className="text-lg font-semibold">{props.title}</h2>
                <p className="text-sm text-muted-foreground">{props.description}</p>
            </div>

            <Separator className="my-4" />

            <div className="grid gap-4 md:grid-cols-2">
                <div className="rounded-lg border bg-background p-4">
                    <h3 className="text-sm font-medium">Configuration</h3>
                    <p className="mt-1 text-sm text-muted-foreground">
                        Config options placeholder
                    </p>
                </div>

                <div className="rounded-lg border bg-background p-4">
                    <h3 className="text-sm font-medium">Preview</h3>
                    <p className="mt-1 text-sm text-muted-foreground">
                        Preview placeholder
                    </p>
                </div>
            </div>
        </div>
    );
}

export default App;