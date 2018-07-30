using System;

namespace ModCore.Logic.EntityFramework
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class EfPropertyBaseAttribute : Attribute
    {
        /// <summary>
        /// If set to false in an implementation of this attribute, will throw an exception when encountering more than
        /// one of the attribute on a type.
        /// </summary>
        public virtual bool CanHaveMultiple => true;
    }
}