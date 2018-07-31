using System;
using System.Linq;
using ModCore.Entities;

namespace ModCore.Logic.EntityFramework.AttributeImpl
{
    /// <inheritdoc />
    /// <summary>
    /// Represents an attribute that is placed on a property to indicate that the database column is to be omitted if
    /// the database provider does not match the given one.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class IgnoreIfProviderNotAttribute : EfPropertyProcessorBaseAttribute
    {
        private readonly DatabaseProvider[] _providers;

        /// <summary>
        /// Initializes a new IgnoreIfProviderNotAttribute instance for one or more providers to match with.
        /// </summary>
        public IgnoreIfProviderNotAttribute(params DatabaseProvider[] providers)
        {
            _providers = providers;
        }

        public override void Process(EfProcessorContext ctx, EfPropertyDefinition definition)
        {
            if (_providers.All(e => e != ctx.DatabaseContext.Provider))
                ctx.Entity.Ignore(definition.Property.Name); // TODO is this correct or do we use the column name here
        }
    }
}