using Disk.Entities;
using Disk.Repository.Common.Interface;

namespace Disk.Repository.Interface
{
    public interface IPathToTargetRepository : ICrudRepository<PathToTarget>
    {
        ICollection<PathToTarget> GetPathsToTargetsBySession(long sessionId);
    }
}
