using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities.Enums
{
    [Flags]
    public enum UserFlags
    {
        Staff = 1 << 0,
        Partner = 1 << 1,
        Hypesquad = 1 << 2,
        BugHunterLevel1 = 1 << 3,
        HypesquadBravery = 1 << 6,
        HypesquadBrilliance = 1 << 7,
        HypesquadBalance = 1 << 8,
        EarlySupporter = 1 << 9,
        IsTeam = 1 << 10,
        BugHunterLevel2 = 1 << 14,
        VerifiedBot = 1 << 16,
        VerifiedDeveloper = 1 << 17,
        CertifiedModerator = 1 << 18,
        BotOnlyUsesHttpInteractions = 1 << 19,
        ActiveDeveloper = 1 << 22
    }
}
