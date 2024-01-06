using ModCore.Common.Discord.Entities.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities.Enums
{
    [JsonConverter(typeof(PermissionsJsonSerializer))]
    [Flags]
    public enum Permissions : long
    {
        None = 0,
        All = 2199023255551,

        CreateInstantInvite = 0x0000000000000001,
        KickMembers = 0x0000000000000002,
        BanMembers = 0x0000000000000004,
        Administrator = 0x0000000000000008,
        ManageChannels = 0x0000000000000010,
        ManageGuild = 0x0000000000000020,
        AddReactions = 0x0000000000000040,
        ViewAuditLog = 0x0000000000000080,
        PrioritySpeaker = 0x0000000000000100,
        Stream = 0x0000000000000200,
        ViewChannel = 0x0000000000000400,
        SendMessages = 0x0000000000000800,
        SendTTSMessages = 0x0000000000001000,
        ManageMessages = 0x0000000000002000,
        EmbedLinks = 0x0000000000004000,
        AttachFiles = 0x0000000000008000,
        ReadMessageHistory = 0x0000000000010000,
        MentionEveryone = 0x0000000000020000,
        UseExternalEmojis = 0x0000000000040000,
        ViewGuildInsights = 0x0000000000080000,
        Connect = 0x0000000000100000,
        Speak = 0x0000000000200000,
        MuteMembers = 0x0000000000400000,
        DeafenMembers = 0x0000000000800000,
        MoveMembers = 0x0000000001000000,
        UseVoiceActivityDetection = 0x0000000002000000,
        ChangeNickname = 0x0000000004000000,
        ManageNicknames = 0x0000000008000000,
        ManageRoles = 0x0000000010000000,
        ManageWebhooks = 0x0000000020000000,
        ManageEmojisStickersSoundsNShit = 0x0000000040000000,
        UseApplicationCommands = 0x0000000080000000,
        RequestToSpeak = 0x0000000100000000,
        ManageEvents = 0x0000000200000000,
        ManageThreads = 0x0000000400000000,
        CreatePublicThreads = 0x0000000800000000,
        CreatePrivateThreads = 0x0000001000000000,
        UseExternalStickers = 0x0000002000000000,
        SendMessagesInThreads = 0x0000004000000000,
        UseEmbeddedActivities = 0x0000008000000000,
        ModerateMembers = 0x0000010000000000,
        ViewCreatorMoneticationAnalytics = 0x0000020000000000,
        UseSoundboard = 0x0000040000000000,
        CreateEmojisStickersSoundsNShit = 0x0000080000000000,
        CreateEvents = 0x0000100000000000,
        UseExternalSounds = 0x0000200000000000,
        SendVoiceMessages = 0x0000400000000000
    }
}
