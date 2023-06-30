using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ModCore.Utils.EntityFramework.AttributeImpl
{
    /// <inheritdoc />
    /// <summary>
    /// Represents an attribute that is placed on a property to append an annotation to it, as with
    /// <see cref="PropertyBuilder{TProperty}.HasAnnotation"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class AnnotationAttribute : EfPropertyProcessorBaseAttribute
    {
        private readonly string _annotation;
        private readonly object _value;

        /// <summary>
        /// Initializes a new AnnotationAttribute instance, with an annotation and a value.
        /// </summary>
        public AnnotationAttribute(string annotation, object value)
        {
            _annotation = annotation;
            _value = value;
        }

        public override void Process(EfProcessorContext ctx, EfPropertyDefinition definition)
        {
            ctx.Entity.Property(definition.Property.Name)
                .HasAnnotation(_annotation, _value);
        }
    }
}