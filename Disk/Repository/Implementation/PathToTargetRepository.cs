using Disk.Db.Context;
using Disk.Entities;
using Disk.Repository.Common.Implementation;
using Disk.Repository.Interface;

namespace Disk.Repository.Implementation
{
    public class PathToTargetRepository(DiskContext diskContext) : CrudRepository<PathToTarget>(diskContext), IPathToTargetRepository
    {
        public ICollection<PathToTarget> GetPathsToTargetsBySession(long sessionId)
        {
            throw new NotImplementedException();
            //return table.Where(p => p.Session == sessionId).ToList();
        }
    }
}
