export type ModCoreWelcomeSettings = {
    channel_id: bigint;
    message_id: string;
    image_b64: string | null;
    x: number;
    y: number;
    width: number;
    height: number;
    shape: WelcomeImageShape;
    enabled: boolean;
};

export enum WelcomeImageShape 
{
    Circle = 1,
    Square = 0,
    Squircle = 2
}