using Disk.Db.Context;
using Disk.Entities;
using Disk.Repository.Common.Implementation;
using Disk.Repository.Interface;

namespace Disk.Repository.Implementation
{
    public class PathInTargetRepository(DiskContext diskContext) : CrudRepository<PathInTarget>(diskContext), IPathInTargetRepository
    {

    }
}
