using Disk.Db.Context;
using Disk.Entities;
using Disk.Repository.Interface;

namespace Disk.Repository.Implementation
{
    public class PathInTargetRepository(DiskContext diskContext) : IPathInTargetRepository
    {
        public void Add(PathInTarget entity)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(PathInTarget entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(PathInTarget entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(PathInTarget entity)
        {
            throw new NotImplementedException();
        }

        public PathInTarget Get(PathInTarget entity)
        {
            throw new NotImplementedException();
        }

        public ICollection<PathInTarget> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<PathInTarget>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PathInTarget> GetAsync(PathInTarget entity)
        {
            throw new NotImplementedException();
        }

        public void Update(PathInTarget entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(PathInTarget entity)
        {
            throw new NotImplementedException();
        }
    }
}
