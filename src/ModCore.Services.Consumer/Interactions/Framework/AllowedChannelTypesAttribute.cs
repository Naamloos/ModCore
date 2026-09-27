using System;
using System.Collections.Generic;
using System.Text;
using ModCore.Common.Discord.Entities.Enums;

namespace ModCore.Services.Consumer.Interactions.Framework
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class AllowedChannelTypesAttribute : Attribute
    {
        public ChannelType[] AllowedTypes { get; private set; }

        public AllowedChannelTypesAttribute(params ChannelType[] allowedTypes)
        {
            AllowedTypes = allowedTypes;
        }
    }
}
