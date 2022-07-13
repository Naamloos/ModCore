using System;

namespace ModCore.Logic.EntityFramework
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class EfPropertyProcessorBaseAttribute : EfPropertyBaseAttribute
    {
        public abstract void Process(EfProcessorContext ctx, EfPropertyDefinition definition);
    }
}