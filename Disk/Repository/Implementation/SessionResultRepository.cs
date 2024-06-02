using Disk.Db.Context;
using Disk.Entities;
using Disk.Repository.Interface;

namespace Disk.Repository.Implementation
{
    public class SessionResultRepository(DiskContext diskContext) : ISesssionResultRepository
    {
        public void Add(SessionResult entity)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(SessionResult entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(SessionResult entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(SessionResult entity)
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

        public SessionResult Get(SessionResult entity)
        {
            throw new NotImplementedException();
        }

        public ICollection<SessionResult> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<SessionResult>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<SessionResult> GetAsync(SessionResult entity)
        {
            throw new NotImplementedException();
        }

        public SessionResult GetById(long id)
        {
            throw new NotImplementedException();
        }

        public Task<SessionResult> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public void Update(SessionResult entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(SessionResult entity)
        {
            throw new NotImplementedException();
        }
    }
}
