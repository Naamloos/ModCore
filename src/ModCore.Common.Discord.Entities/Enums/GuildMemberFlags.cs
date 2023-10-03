namespace ModCore.Common.Discord.Entities.Enums
{
    [Flags]
    public enum GuildMemberFlags
    {
        DidRejoin = 1<<0,
        CompletedOnboarding = 1<<1,
        BypassesVerification = 1<<2,
        StartedOnboarding = 1<<3
    }
}
