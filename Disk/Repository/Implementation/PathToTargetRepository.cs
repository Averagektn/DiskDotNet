using Disk.Db.Context;
using Disk.Entities;
using Disk.Repository.Interface;

namespace Disk.Repository.Implementation
{
    public class PathToTargetRepository(DiskContext diskContext) : IPathToTargetRepository
    {
        public void Add(PathToTarget entity)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(PathToTarget entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(PathToTarget entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(PathToTarget entity)
        {
            throw new NotImplementedException();
        }

        public ICollection<PathToTarget> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<PathToTarget>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public PathToTarget GetById(long id)
        {
            throw new NotImplementedException();
        }

        public Task<PathToTarget> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public void Update(PathToTarget entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(PathToTarget entity)
        {
            throw new NotImplementedException();
        }
    }
}
