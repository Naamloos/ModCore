using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ModCore.Logic.EntityFramework
{
    public sealed class EfProcessorContext
    {
        public ModelBuilder Model { get; internal set; }
        public EntityTypeBuilder Entity { get; internal set; }
        public IMutableEntityType EntityType { get; internal set; }
    }
}