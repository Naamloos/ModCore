using ModCore.Entities;

namespace ModCore.Utils.EntityFramework
{
    public interface IEfCustomContext
    {
        DatabaseProvider Provider { get; }
    }
}