using Disk.Db.Context;
using Disk.Entities;
using Disk.Repository.Interface;

namespace Disk.Repository.Implementation
{
    public class MapRepository(DiskContext diskContext) : IMapRepository
    {
        public void Add(Map entity)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(Map entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Map entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Map entity)
        {
            throw new NotImplementedException();
        }

        public void DeleteById(long id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Map Get(Map entity)
        {
            throw new NotImplementedException();
        }

        public ICollection<Map> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Map>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Map> GetAsync(Map entity)
        {
            throw new NotImplementedException();
        }

        public Map GetById(long id)
        {
            throw new NotImplementedException();
        }

        public Task<Map> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public void Update(Map entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Map entity)
        {
            throw new NotImplementedException();
        }
    }
}
