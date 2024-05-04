using Disk.Db.Context;
using Disk.Entity;

namespace Disk.Repository
{
    public class StaticticsRepository(DiskContext context)
    {
        public async Task<int> StartSession(Session session)
        {
            throw new NotImplementedException();
        }

        public async Task AddStatistics(int sessionId, SessionResult sessionResult, PathInTarget pathInTarget, 
            PathToTarget pathToTarget)
        {
            throw new NotImplementedException();
        }
    }
}
