export default interface ModCoreInfraction {
    guild_id: string;
    user_id: string;
    responsible_moderator_id: string;
    reason: string;
    user_was_notified: boolean;
    infraction_type: InfractionType;
}

export enum InfractionType {
    Warning,
    Ban,
    Kick,
    Mute,
    TempBan,
    SoftBan,
    HackBan,
    MassBan,
    Isolate,
    Appealed,
    AppealDenied,
    VoiceBan
}
