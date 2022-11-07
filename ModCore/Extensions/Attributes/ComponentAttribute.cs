using DSharpPlus;
using System;

namespace ModCore.Extensions.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ComponentAttribute : Attribute
    {
        public string Id { get; init; }
        public ComponentType ComponentType { get; init; }

        public ComponentAttribute(string id, ComponentType componentType)
        {
            this.Id = id;
            this.ComponentType = componentType;
        }
    }
}
