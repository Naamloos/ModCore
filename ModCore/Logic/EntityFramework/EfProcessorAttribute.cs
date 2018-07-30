using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ModCore.Logic.EntityFramework
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class EfPropertyBaseAttribute : Attribute
    {
        public abstract bool IsUnique { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class EfPropertyProcessorBaseAttribute : EfPropertyBaseAttribute
    {
        public abstract void Process(EfProcessorContext ctx, EfPropertyDefinition definition);
    }

    /// <summary>
    /// Represents a type processor that is applied as an attribute to one or more properties. Properties where
    /// <see cref="PropertyMatches"/> returns true will be aggregated, and then one instance of the attribute will have
    /// <see cref="Process"/> called on it with the aggregated properties. If there are no matches, the method will not
    /// be called. 
    /// </summary>
    /// <remarks>
    /// <p>There is no guarantee as to which instance of the attribute will be used for processing. Implementations of this
    /// class should be stateless and sealed.</p>
    /// <p>The choice of having the same attribute type handling both properties and containing types is to avoid
    /// needing to use reflection.</p>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class EfIndirectProcessorBaseAttribute : EfPropertyBaseAttribute
    {
        public abstract bool PropertyMatches(EfProcessorContext ctx, EfPropertyDefinition definition);
        public abstract void Process(EfProcessorContext ctx, IReadOnlyCollection<EfPropertyDefinition> properties);

        // only compare equality and hash code based on the own type and prevent overriding it
        // this makes it possible to index attributes as keys in a dictionary
        public sealed override bool Equals(object obj) => obj?.GetType() == GetType();
        public sealed override int GetHashCode() => GetType().GetHashCode();
    }

    public class EfProcessorContext
    {
        public ModelBuilder Model { get; set; }
        public EntityTypeBuilder Entity { get; set; }
        public IMutableEntityType EntityType { get; set; }
    }
    
    public class EfPropertyDefinition
    {
        public PropertyInfo Property { get; set; }
        public Attribute Source { get; set; }
    }
}