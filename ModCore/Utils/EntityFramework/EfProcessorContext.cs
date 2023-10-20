using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ModCore.Utils.EntityFramework
{
    public sealed class EfProcessorContext
    {
        public ModelBuilder Model { get; internal set; }
        public EntityTypeBuilder Entity { get; internal set; }
        public IMutableEntityType EntityType { get; internal set; }
        public IEfCustomContext DatabaseContext { get; internal set; }
        
        public Type ClrType => EntityType.ClrType;
    }
}