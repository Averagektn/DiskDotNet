using Disk.Db.Context;
using Disk.Entities;
using Disk.Repository.Common.Implementation;
using Disk.Repository.Interface;

namespace Disk.Repository.Implementation;

public class PathToTargetRepository(DiskContext diskContext) : CrudRepository<PathToTarget>(diskContext), IPathToTargetRepository
{
    public ICollection<PathToTarget> GetPathsToTargetsBySession(long sessionId)
    {
        return [.. Table.Where(p => p.Session == sessionId)];
    }
}
