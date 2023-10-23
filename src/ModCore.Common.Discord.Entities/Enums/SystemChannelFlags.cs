using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities.Enums
{
    [Flags]
    public enum SystemChannelFlags
    {
        SuppressJoinNotifications = 1 << 0,
        SuppressPremiumSubscriptions = 1 << 1,
        SuppressGuildReminderNotifications = 1 << 2,
        SuppressJoinNotificationReplies = 1 << 3,
        SuppressRoleSubscriptionPurchaseNotifications = 1 << 4,
        SuppressRoleSubscriptionPurchaseNotificationReplies = 1 << 5
    }
}
