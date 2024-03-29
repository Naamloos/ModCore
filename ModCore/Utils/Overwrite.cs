﻿using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

namespace ModCore.Utils
{
    public class Overwrite : ModCoreSnowflake
    {
        /// <summary>
        /// Gets the type of the overwrite.
        /// </summary>
        public OverwriteType Type { get; internal set; }

        /// <summary>Gets the allowed permission set.</summary>
        public Permissions Allow { get; internal set; }

        /// <summary>Gets the denied permission set.</summary>
        public Permissions Deny { get; internal set; }

        public Task<DiscordMember> MemberGetterAsync =>
            Type == OverwriteType.Member ? _guild.GetMemberAsync(Id) : throw new ArgumentException();

        public DiscordRole Role => Type == OverwriteType.Role ? _guild.GetRole(Id) : throw new ArgumentException();

        private readonly DiscordGuild _guild;

        public Overwrite(DiscordOverwrite overwrite, DiscordGuild guild)
        {
            this.Type = overwrite.Type;
            this.Allow = overwrite.Allowed;
            this.Deny = overwrite.Denied;
            this.Id = overwrite.Id;
            _guild = guild;
        }
    }
}