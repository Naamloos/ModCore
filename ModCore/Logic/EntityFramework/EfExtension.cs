// some snippets of code are from jsakamoto/EntityFrameworkCore.IndexAttribute

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
using System.Threading.Tasks;
using Microsoft.Collections.Extensions;
using Microsoft.EntityFrameworkCore;
using ModCore.Database;

namespace ModCore.Logic.EntityFramework
{
    public static class EfExtension
    {
        public static ModelBuilder BuildCustomAttributes(this ModelBuilder model, IEfCustomContext dbContext)
        {
            var entityTypes = model.Model.GetEntityTypes();
            var modelChangeLock = new object();

            Parallel.ForEach(entityTypes, entityType =>
            {
                // matches, ctx and the tally are lazily loaded to avoid computing when no actual matching attributes
                // exist.
                MultiValueDictionary<EfIndirectProcessorBaseAttribute, EfPropertyDefinition> matches = null;
                EfProcessorContext ctx = null;
                ISet<Type> uniqueAttributeTally = null;
                
                foreach (var prop in entityType.ClrType.GetProperties())
                {
                    //TODO does this actually include types inheriting that type?
                    var attrs = Attribute.GetCustomAttributes(prop, typeof(EfPropertyBaseAttribute));
                    
                    // note to self: we don't restart the tally for every new property, that ruins the entire point, we
                    // have AttributeUsage.AllowMultiple for that
                    
                    foreach (var attr in attrs)
                    {
                        // lazily initialize the context (so we don't do model.Entity when we don't need to)
                        if (ctx == null)
                        {
                            ctx = new EfProcessorContext
                            {
                                Model = model,
                                Entity = model.Entity(entityType.ClrType),
                                EntityType = entityType,
                                DatabaseContext = dbContext,
                            };
                        }

                        // this is better kept local
                        var definition = new EfPropertyDefinition
                        {
                            Property = prop,
                            Source = attr,
                        };

                        // we need the EfPropertyBaseAttribute value, but we can't do that inside the switch block, so
                        // we're using pattern matching here
                        if (!(attr is EfPropertyBaseAttribute baseAttr))
                            continue;

                        if (!baseAttr.CanHaveMultiple)
                        {
                            if (uniqueAttributeTally == null)
                            {
                                // lazily initialize the tally
                                // if there is no tally, there is no need to check if it contains the type
                                uniqueAttributeTally = new HashSet<Type> {baseAttr.GetType()};
                            }
                            else if (!uniqueAttributeTally.Add(baseAttr.GetType()))
                            {
                                throw new EfAttributeException(
                                    $"The property {prop} contains multiple instances of {baseAttr.GetType()}");
                            }
                        }
                        
                        switch (attr)
                        {
                            case EfPropertyProcessorBaseAttribute processor:
                                lock (modelChangeLock)
                                {
                                    processor.Process(ctx, definition);
                                }
                                break;
                            case EfIndirectProcessorBaseAttribute indirectProcessor:
                                if (indirectProcessor.PropertyMatches(ctx, definition))
                                {
                                    if (matches == null)
                                        matches = new MultiValueDictionary<EfIndirectProcessorBaseAttribute, EfPropertyDefinition>();
                                    matches.Add(indirectProcessor, definition);
                                }
                                break;
                        }
                    }
                }

                if (matches != null)
                {
                    lock (modelChangeLock)
                    {
                        foreach (var (attr, defs) in matches)
                        {
                            attr.Process(ctx, defs);
                        }
                    }
                }
            });

            // allow chaining
            return model;
        }
    }
}