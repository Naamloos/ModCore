using System;

namespace ModCore.Utils.EntityFramework
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class EfPropertyProcessorBaseAttribute : EfPropertyBaseAttribute
    {
        public abstract void Process(EfProcessorContext ctx, EfPropertyDefinition definition);
    }
}