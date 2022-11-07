using DSharpPlus;
using System;

namespace ModCore.Extensions.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ComponentPermissionsAttribute : Attribute
    {
        public Permissions Permissions { get; init; }

        public ComponentPermissionsAttribute(Permissions permissions)
        {
            Permissions = permissions;
        }
    }
}
