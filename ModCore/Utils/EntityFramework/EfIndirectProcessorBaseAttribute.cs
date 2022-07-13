using System;
using System.Collections.Generic;

namespace ModCore.Logic.EntityFramework
{
    /// <summary>
    /// Represents a type processor that is applied as an attribute to one or more properties. Properties where
    /// <see cref="PropertyMatches"/> returns true will be aggregated, and then one instance of the attribute will have
    /// <see cref="Process"/> called on it with the aggregated properties. If there are no matches, the method will not
    /// be called. 
    /// </summary>
    /// <remarks>
    /// <p>There is no guarantee as to which instance of the attribute will be used for processing. Implementations of
    /// this class can not be compared by their state, and should be sealed.</p>
    /// <p>The choice of having the same attribute type handling both properties and containing types is to avoid
    /// needing to use reflection.</p>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class EfIndirectProcessorBaseAttribute : EfPropertyBaseAttribute
    {
        // all implementations of this class must support multiple annotations per type, otherwise using it is pointless
        public sealed override bool CanHaveMultiple => true;
        
        public abstract bool PropertyMatches(EfProcessorContext ctx, EfPropertyDefinition definition);
        public abstract void Process(EfProcessorContext ctx, IEnumerable<EfPropertyDefinition> properties);

        // only compare equality and hash code based on the own type and prevent overriding it
        // this makes it possible to index attributes as keys in a dictionary
        public sealed override bool Equals(object obj) => obj?.GetType() == GetType();
        public sealed override int GetHashCode() => GetType().GetHashCode();
    }
}