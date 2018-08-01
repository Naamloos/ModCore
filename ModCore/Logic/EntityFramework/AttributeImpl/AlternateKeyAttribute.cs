using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ModCore.Entities;

namespace ModCore.Logic.EntityFramework.AttributeImpl
{
    /// <inheritdoc />
    /// <summary>
    /// Represents an attribute that is placed on a property to indicate that the property is an alternate key in the
    /// model. This will make its properties be unique and read-only.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class AlternateKeyAttribute : EfPropertyProcessorBaseAttribute
    {
        private readonly string _name;

        /// <summary>
        /// Initializes a new AlternateKeyAttribute instance, with an optional name for the key.
        /// </summary>
        public AlternateKeyAttribute(string name = null)
        {
            _name = name;
        }

        public override void Process(EfProcessorContext ctx, EfPropertyDefinition definition)
        {
            var k = ctx.Entity.HasAlternateKey(definition.Property.Name);
            if (_name != null) k.HasName(_name); // TODO i dont think this matters
        }
    }
}