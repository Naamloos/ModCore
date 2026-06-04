import { create } from 'zustand';
import {
    DiscordSDK,
    Common,
    type CommandResponseTypes,
} from '@discord/embedded-app-sdk';

const DISCORD_CLIENT_ID = import.meta.env.VITE_DISCORD_CLIENT_ID ?? '811197813043494942';

type AuthResponse = CommandResponseTypes['authenticate'];

type OrientationUnion = 'UNLOCKED' | 'PORTRAIT' | 'LANDSCAPE';

type LayoutModeName = 'UNKNOWN' | 'FOCUSED' | 'PIP' | 'GRID';

type ThermalStateName = 'UNKNOWN' | 'NOMINAL' | 'FAIR' | 'SERIOUS' | 'CRITICAL';

type SetOrientationOptions = {
    focused?: OrientationUnion;
    pip?: OrientationUnion | null;
    grid?: OrientationUnion | null;
};

type DashboardActivityOptions = {
    details?: string;
    state?: string;
    largeImage?: string;
    largeText?: string;
    smallImage?: string;
    smallText?: string;
};

type ActivityLayoutModeUpdateEvent = {
    layout_mode: unknown;
};

type OrientationUpdateEvent = {
    screen_orientation: unknown;
};

type ThermalStateUpdateEvent = {
    thermal_state: unknown;
};

type ParticipantsUpdateEvent = {
    participants?: unknown[];
};

type CurrentUserUpdateEvent = unknown;

type CurrentGuildMemberUpdateEvent = unknown;

type DiscordSdkEventListeners = {
    activityLayoutModeUpdate?: (event: ActivityLayoutModeUpdateEvent) => void;
    orientationUpdate?: (event: OrientationUpdateEvent) => void;
    thermalStateUpdate?: (event: ThermalStateUpdateEvent) => void;
    currentUserUpdate?: (event: CurrentUserUpdateEvent) => void;
    currentGuildMemberUpdate?: (event: CurrentGuildMemberUpdateEvent) => void;
    participantsUpdate?: (event: ParticipantsUpdateEvent) => void;
};

interface DiscordState {
    sdk: DiscordSDK | null;
    authResponse: AuthResponse | null;

    isReady: boolean;
    isAuthenticated: boolean;
    isInitializing: boolean;
    isMinimized: boolean;
    isDoneLoading: boolean;

    error: string | null;

    channelId: string | null;
    guildId: string | null;
    instanceId: string | null;

    layoutMode: LayoutModeName;
    rawLayoutMode: unknown;

    screenOrientation: unknown;

    thermalState: ThermalStateName;
    rawThermalState: unknown;

    currentUser: unknown | null;
    currentGuildMember: unknown | null;
    currentGuild: any | null;
    participants: unknown[];

    locale: string | null;

    initialize: () => Promise<AuthResponse>;
    reset: () => Promise<void>;

    requireSdk: () => DiscordSDK;
    requireGuildId: () => string;
    requireChannelId: () => string;

    setOrientation: (lockState: OrientationUnion) => Promise<void>;
    setOrientationLockState: (options: SetOrientationOptions) => Promise<void>;

    setInteractivePip: (enabled: boolean) => Promise<void>;

    refreshChannel: () => Promise<unknown | null>;
    getChannelPermissions: () => Promise<unknown | null>;
    refreshParticipants: () => Promise<unknown | null>;
    refreshLocale: () => Promise<unknown | null>;
    getPlatformBehaviors: () => Promise<unknown | null>;

    getEntitlements: () => Promise<unknown | null>;
    getSkus: () => Promise<unknown | null>;

    openInviteDialog: () => Promise<void>;
    openExternalLink: (url: string) => Promise<void>;
    shareLink: (message?: string, customId?: string) => Promise<unknown | null>;
    initiateImageUpload: () => Promise<unknown | null>;

    setDashboardActivity: (options?: DashboardActivityOptions) => Promise<void>;

    captureLog: (
        level: 'log' | 'warn' | 'error' | 'debug' | 'info',
        message: string
    ) => Promise<void>;

    closeActivity: (code?: number, reason?: string) => void;
}

let setupPromise: Promise<AuthResponse> | null = null;
let currentSdkInstance: DiscordSDK | null = null;
let activeEventListeners: DiscordSdkEventListeners = {};

const safeErrorMessage = (error: unknown): string => {
    if (error instanceof Error) return error.message;
    return 'Unknown Discord SDK error';
};

const getSdkOrientationLockValue = (value: OrientationUnion) => {
    return Common.OrientationLockStateTypeObject[value];
};

const getLayoutModeName = (layoutMode: unknown): LayoutModeName => {
    const commonAny = Common as any;

    const layoutEnum =
        commonAny.LayoutModeTypeObject ??
        commonAny.ActivityLayoutModeTypeObject ??
        commonAny.ActivityLayoutMode;

    if (layoutEnum) {
        if (
            layoutMode === layoutEnum.FOCUSED ||
            layoutMode === layoutEnum.FOCUS ||
            layoutMode === layoutEnum.DEFAULT
        ) {
            return 'FOCUSED';
        }

        if (
            layoutMode === layoutEnum.PIP ||
            layoutMode === layoutEnum.PICTURE_IN_PICTURE
        ) {
            return 'PIP';
        }

        if (layoutMode === layoutEnum.GRID) {
            return 'GRID';
        }
    }

    /**
     * Fallback for SDK versions where the layout enum is not exported.
     * Discord has shown ACTIVITY_LAYOUT_MODE_UPDATE as numeric in examples.
     */
    switch (layoutMode) {
        case 0:
            return 'FOCUSED';
        case 1:
            return 'PIP';
        case 2:
            return 'GRID';
        default:
            return 'UNKNOWN';
    }
};

const getThermalStateName = (thermalState: unknown): ThermalStateName => {
    if (thermalState === Common.ThermalStateTypeObject.NOMINAL) return 'NOMINAL';
    if (thermalState === Common.ThermalStateTypeObject.FAIR) return 'FAIR';
    if (thermalState === Common.ThermalStateTypeObject.SERIOUS) return 'SERIOUS';
    if (thermalState === Common.ThermalStateTypeObject.CRITICAL) return 'CRITICAL';

    return 'UNKNOWN';
};

const unsubscribeFromSdkEvents = async (sdk: DiscordSDK): Promise<void> => {
    const listeners = activeEventListeners;

    await Promise.allSettled([
        listeners.activityLayoutModeUpdate
            ? sdk.unsubscribe(
                'ACTIVITY_LAYOUT_MODE_UPDATE',
                listeners.activityLayoutModeUpdate
            )
            : Promise.resolve(),

        listeners.orientationUpdate
            ? sdk.unsubscribe('ORIENTATION_UPDATE', listeners.orientationUpdate)
            : Promise.resolve(),

        listeners.thermalStateUpdate
            ? sdk.unsubscribe('THERMAL_STATE_UPDATE', listeners.thermalStateUpdate)
            : Promise.resolve(),

        listeners.currentUserUpdate
            ? sdk.unsubscribe('CURRENT_USER_UPDATE', listeners.currentUserUpdate)
            : Promise.resolve(),

        listeners.currentGuildMemberUpdate
            ? sdk.unsubscribe(
                'CURRENT_GUILD_MEMBER_UPDATE',
                listeners.currentGuildMemberUpdate,
                { guild_id: sdk.guildId! }
            )
            : Promise.resolve(),

        listeners.participantsUpdate
            ? sdk.unsubscribe(
                'ACTIVITY_INSTANCE_PARTICIPANTS_UPDATE',
                listeners.participantsUpdate
            )
            : Promise.resolve(),
    ]);

    activeEventListeners = {};
};

const subscribeToSdkEvents = async (
    sdk: DiscordSDK,
    set: (partial: Partial<DiscordState>) => void
): Promise<void> => {
    await unsubscribeFromSdkEvents(sdk);

    const activityLayoutModeUpdate = (event: ActivityLayoutModeUpdateEvent) => {
        const layoutMode = getLayoutModeName(event.layout_mode);

        set({
            rawLayoutMode: event.layout_mode,
            layoutMode,
            isMinimized: layoutMode === 'PIP',
        });
    };

    const orientationUpdate = (event: OrientationUpdateEvent) => {
        set({
            screenOrientation: event.screen_orientation,
        });
    };

    const thermalStateUpdate = (event: ThermalStateUpdateEvent) => {
        set({
            rawThermalState: event.thermal_state,
            thermalState: getThermalStateName(event.thermal_state),
        });
    };

    const currentUserUpdate = (event: CurrentUserUpdateEvent) => {
        set({
            currentUser: event,
        });
    };

    const currentGuildMemberUpdate = (event: CurrentGuildMemberUpdateEvent) => {
        set({
            currentGuildMember: event,
        });
    };

    const participantsUpdate = (event: ParticipantsUpdateEvent) => {
        set({
            participants: event.participants ?? [],
        });
    };

    activeEventListeners = {
        activityLayoutModeUpdate,
        orientationUpdate,
        thermalStateUpdate,
        currentUserUpdate,
        currentGuildMemberUpdate,
        participantsUpdate,
    };

    const subscriptions: Promise<unknown>[] = [
        sdk.subscribe('ACTIVITY_LAYOUT_MODE_UPDATE', activityLayoutModeUpdate),
        sdk.subscribe('ORIENTATION_UPDATE', orientationUpdate),
        sdk.subscribe('THERMAL_STATE_UPDATE', thermalStateUpdate),
        sdk.subscribe('CURRENT_USER_UPDATE', currentUserUpdate),
        sdk.subscribe('ACTIVITY_INSTANCE_PARTICIPANTS_UPDATE', participantsUpdate),
    ];

    if (sdk.guildId) {
        subscriptions.push(
            sdk.subscribe(
                'CURRENT_GUILD_MEMBER_UPDATE',
                currentGuildMemberUpdate,
                {
                    guild_id: sdk.guildId,
                }
            )
        );
    }

    await Promise.all(subscriptions);
};

export const useDiscordStore = create<DiscordState>((set, get) => {
    const initialize = async (): Promise<AuthResponse> => {
        if (setupPromise) {
            return setupPromise;
        }

        setupPromise = (async () => {
            set({
                isInitializing: true,
                error: null,
            });

            try {
                if (!DISCORD_CLIENT_ID) {
                    throw new Error('Missing Discord client ID');
                }

                const sdk = new DiscordSDK(DISCORD_CLIENT_ID);
                currentSdkInstance = sdk;

                await sdk.ready();

                set({
                    sdk,
                    isReady: true,
                    channelId: sdk.channelId ?? null,
                    guildId: sdk.guildId ?? null,
                    instanceId: sdk.instanceId ?? null,
                });

                const { code } = await sdk.commands.authorize({
                    client_id: DISCORD_CLIENT_ID,
                    response_type: 'code',
                    state: crypto.randomUUID(),
                    prompt: 'none',
                    scope: [
                        'identify',
                        'guilds',

                        /**
                         * Useful for dashboard/member context.
                         * Your backend should still verify actual bot dashboard permissions.
                         */
                        'guilds.members.read',

                        /**
                         * Needed for setActivity.
                         * Remove this if you do not use setDashboardActivity().
                         */
                        'rpc.activities.write',
                    ],
                });

                const tokenResponse = await fetch('/api/token', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({ code }),
                });

                if (!tokenResponse.ok) {
                    throw new Error(
                        `Token exchange failed with status: ${tokenResponse.status}`
                    );
                }

                const tokenPayload = await tokenResponse.json();

                if (!tokenPayload.access_token) {
                    throw new Error('Token exchange did not return an access_token');
                }

                const authPayload = await sdk.commands.authenticate({
                    access_token: tokenPayload.access_token,
                });

                await subscribeToSdkEvents(sdk, set);

                set({
                    sdk,
                    authResponse: authPayload,
                    isAuthenticated: true,
                    error: null,
                });

                // 1. From the HTTP API fetch a list of all of the user's guilds
                const guilds = await fetch(`https://discord.com/api/v10/users/@me/guilds`, {
                    headers: {
                        // NOTE: we're using the access_token provided by the "authenticate" command
                        Authorization: `Bearer ${tokenPayload.access_token}`,
                        'Content-Type': 'application/json',
                    },
                }).then((response) => response.json());

                // 2. Find the current guild's info, including it's "icon"
                // @ts-ignore
                set({
                    currentGuild: guilds.find((g) => g.id === sdk.guildId),
                    isInitializing: false,
                    isDoneLoading: true,
                });

                /**
                 * Prime useful dashboard state.
                 * These are intentionally non-fatal.
                 */
                await Promise.allSettled([
                    get().refreshLocale(),
                    get().refreshParticipants(),
                    get().refreshChannel(),
                ]);

                return authPayload;
            } catch (error) {
                setupPromise = null;
                currentSdkInstance = null;

                set({
                    sdk: null,
                    authResponse: null,

                    isReady: false,
                    isAuthenticated: false,
                    isInitializing: false,
                    isMinimized: false,
                    isDoneLoading: false,

                    error: safeErrorMessage(error),

                    channelId: null,
                    guildId: null,
                    instanceId: null,

                    layoutMode: 'UNKNOWN',
                    rawLayoutMode: null,

                    screenOrientation: null,

                    thermalState: 'UNKNOWN',
                    rawThermalState: null,

                    currentUser: null,
                    currentGuildMember: null,
                    participants: [],

                    locale: null,
                });

                throw error;
            }
        })();

        return setupPromise;
    };

    /**
     * Eager initialization.
     * You can remove this if you prefer manually calling initialize()
     * from a React bootstrap component.
     */
    initialize().catch((error) => {
        console.error('Auto-initialization of Discord SDK failed:', error);
    });

    return {
        sdk: null,
        authResponse: null,

        isReady: false,
        isAuthenticated: false,
        isInitializing: false,
        isMinimized: false,
        isDoneLoading: false,

        error: null,

        channelId: null,
        guildId: null,
        instanceId: null,

        layoutMode: 'UNKNOWN',
        rawLayoutMode: null,

        screenOrientation: null,

        thermalState: 'UNKNOWN',
        rawThermalState: null,

        currentUser: null,
        currentGuildMember: null,
        participants: [],
        currentGuild: null,

        locale: null,

        initialize,

        reset: async () => {
            const sdk = get().sdk ?? currentSdkInstance;

            if (sdk) {
                await unsubscribeFromSdkEvents(sdk);
            }

            setupPromise = null;
            currentSdkInstance = null;

            set({
                sdk: null,
                authResponse: null,

                isReady: false,
                isAuthenticated: false,
                isInitializing: false,
                isMinimized: false,

                error: null,

                channelId: null,
                guildId: null,
                instanceId: null,

                layoutMode: 'UNKNOWN',
                rawLayoutMode: null,

                screenOrientation: null,

                thermalState: 'UNKNOWN',
                rawThermalState: null,

                currentUser: null,
                currentGuildMember: null,
                participants: [],

                locale: null,
            });
        },

        requireSdk: () => {
            const sdk = get().sdk;

            if (!sdk) {
                throw new Error('Discord SDK has not been initialized yet');
            }

            return sdk;
        },

        requireGuildId: () => {
            const sdk = get().requireSdk();

            if (!sdk.guildId) {
                throw new Error('This Discord Activity is not running inside a guild.');
            }

            return sdk.guildId;
        },

        requireChannelId: () => {
            const sdk = get().requireSdk();

            if (!sdk.channelId) {
                throw new Error('This Discord Activity does not have a channel ID.');
            }

            return sdk.channelId;
        },

        setOrientation: async (lockState) => {
            await get().setOrientationLockState({
                focused: lockState,
                pip: lockState,
                grid: lockState,
            });
        },

        setOrientationLockState: async ({
            focused = 'UNLOCKED',
            pip,
            grid,
        }) => {
            const sdk = get().requireSdk();

            await sdk.commands.setOrientationLockState({
                lock_state: getSdkOrientationLockValue(focused),

                /**
                 * undefined: leave existing SDK value unchanged
                 * null: clear layout-specific override
                 */
                picture_in_picture_lock_state:
                    pip === undefined
                        ? undefined
                        : pip === null
                            ? null
                            : getSdkOrientationLockValue(pip),

                grid_lock_state:
                    grid === undefined
                        ? undefined
                        : grid === null
                            ? null
                            : getSdkOrientationLockValue(grid),
            });
        },

        setInteractivePip: async (enabled) => {
            const sdk = get().requireSdk();

            await sdk.commands.setConfig({
                use_interactive_pip: enabled,
            });
        },

        refreshChannel: async () => {
            const sdk = get().requireSdk();

            if (!sdk.channelId) {
                return null;
            }

            return sdk.commands.getChannel({
                channel_id: sdk.channelId,
            });
        },

        getChannelPermissions: async () => {
            const sdk = get().requireSdk();

            if (!sdk.channelId || !sdk.guildId) {
                return null;
            }

            return sdk.commands.getChannelPermissions();
        },

        refreshParticipants: async () => {
            const sdk = get().requireSdk();

            const response = await sdk.commands.getInstanceConnectedParticipants();

            const participants =
                (response as any)?.participants ??
                (Array.isArray(response) ? response : []);

            set({
                participants,
            });

            return response;
        },

        refreshLocale: async () => {
            const sdk = get().requireSdk();

            const response = await sdk.commands.userSettingsGetLocale();

            const locale =
                typeof response === 'string'
                    ? response
                    : (response as any)?.locale ?? null;

            set({
                locale,
            });

            return response;
        },

        getPlatformBehaviors: async () => {
            const sdk = get().requireSdk();

            return sdk.commands.getPlatformBehaviors();
        },

        getEntitlements: async () => {
            const sdk = get().requireSdk();

            if (!sdk.commands.getEntitlements) {
                return null;
            }

            return sdk.commands.getEntitlements();
        },

        getSkus: async () => {
            const sdk = get().requireSdk();

            if (!sdk.commands.getSkus) {
                return null;
            }

            return sdk.commands.getSkus();
        },

        openInviteDialog: async () => {
            const sdk = get().requireSdk();

            await sdk.commands.openInviteDialog();
        },

        openExternalLink: async (url) => {
            const sdk = get().requireSdk();

            await sdk.commands.openExternalLink({
                url,
            });
        },

        shareLink: async (
            message = 'Open this dashboard in Discord',
            customId = 'dashboard_share'
        ) => {
            const sdk = get().requireSdk();

            if (!sdk.commands.shareLink) {
                return null;
            }

            return sdk.commands.shareLink({
                message,
                custom_id: customId,
            });
        },

        initiateImageUpload: async () => {
            const sdk = get().requireSdk();

            if (!sdk.commands.initiateImageUpload) {
                return null;
            }

            return sdk.commands.initiateImageUpload();
        },

        setDashboardActivity: async ({
            details = 'Using the dashboard',
            state = 'Managing a server',
            largeImage,
            largeText,
            smallImage,
            smallText,
        } = {}) => {
            const sdk = get().requireSdk();

            await sdk.commands.setActivity({
                activity: {
                    type: 0,
                    details,
                    state,
                    assets: {
                        large_image: largeImage,
                        large_text: largeText,
                        small_image: smallImage,
                        small_text: smallText,
                    },
                },
            });
        },

        captureLog: async (level, message) => {
            const sdk = get().requireSdk();

            if (!sdk.commands.captureLog) {
                console[level === 'debug' ? 'debug' : level](message);
                return;
            }

            await sdk.commands.captureLog({
                level,
                message,
            });
        },

        closeActivity: (code = 1000, reason = 'Closed by user') => {
            const sdk = get().requireSdk();

            sdk.close(code, reason);
        },
    };
});