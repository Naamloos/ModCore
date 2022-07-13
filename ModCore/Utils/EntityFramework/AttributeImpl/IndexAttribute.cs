// mostly taken from from jsakamoto/EntityFrameworkCore.IndexAttribute

// MIT License
// 
// Copyright (c) 2017 J.Sakamoto
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace ModCore.Logic.EntityFramework.AttributeImpl
{
    /// <inheritdoc />
    /// <summary>
    /// Represents an attribute that is placed on a property to indicate that the database column to which the property is mapped has an index.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class IndexAttribute : EfIndirectProcessorBaseAttribute
    {
        private readonly struct IndexParam
        {
            public string IndexName { get; }
            public bool IsUnique { get; }
            public bool IsLocal { get; }
            public string[] PropertyNames { get; }

            public IndexParam(IndexAttribute indexAttr, params PropertyInfo[] properties)
            {
                this.IndexName = indexAttr.Name;
                this.IsUnique = indexAttr.IsUnique;
                this.IsLocal = indexAttr.IsLocal;
                this.PropertyNames = properties.Select(prop => prop.Name).ToArray();
            }
        }
        
        /// <summary>
        /// Gets or sets the index name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets a number that determines the column ordering for multi-column indexes. This will be -1 if no column order has been specified.
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// Gets or sets a value to indicate whether to define a unique index.
        /// </summary>
        public bool IsUnique { get; set; }

        /// <summary>
        /// Gets or sets a value to indicate whether to append the enclosing table's name to this index's name, along
        /// with a delimiter. Has no effect if no name is set.
        /// </summary>
        public bool IsLocal { get; set; }

        /// <summary>
        /// Initializes a new IndexAttribute instance for an index with the given name and column order, but with no uniqueness specified.
        /// </summary>
        /// <param name="name">The index name.</param>
        /// <param name="order">A number which will be used to determine column ordering for multi-column indexes.</param>
        public IndexAttribute(string name = null, int order = -1)
        {
            this.Name = name;
            this.Order = order;
        }

        public override bool PropertyMatches(EfProcessorContext ctx, EfPropertyDefinition definition) => true;

        public override void Process(EfProcessorContext ctx, IEnumerable<EfPropertyDefinition> properties)
        {
            var indexParams = properties
                .Select(def => (prop: def.Property, attr: def.Source as IndexAttribute))
                .GroupBy(item => item.attr.Name)
                .Select(group =>
                {
                    var first = group.First();
                    if (group.Key == null)
                    {
                        return new IndexParam(first.attr, first.prop);
                    }

                    return new IndexParam(
                        first.attr,
                        group.OrderBy(item => item.attr.Order).Select(item => item.prop).ToArray());
                });

            string prefix = null;
            
            foreach (var indexParam in indexParams)
            {
                var indexBuilder = ctx.Entity
                    .HasIndex(indexParam.PropertyNames)
                    .IsUnique(indexParam.IsUnique);
                if (indexParam.IndexName != null)
                {
                    if (indexParam.IsLocal && prefix == null)
                    {
                        prefix = ctx.ClrType.GetCustomAttribute<TableAttribute>()?.Name
                                 ?? throw new EfAttributeException("");
                        
                        indexBuilder.HasDatabaseName($"{prefix}_{indexParam.IndexName}");
                    }
                    else
                    {
                        indexBuilder.HasDatabaseName(indexParam.IndexName);
                    }
                }
            }
        }
    }
}