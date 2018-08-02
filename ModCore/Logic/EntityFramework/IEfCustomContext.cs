using ModCore.Entities;

namespace ModCore.Logic.EntityFramework
{
    public interface IEfCustomContext
    {
        DatabaseProvider Provider { get; }
    }
}