using Disk.Db.Context;
using Disk.Entities;
using Disk.Repository.Interface;

namespace Disk.Repository.Implementation
{
    public class SessionRepository(DiskContext diskContext) : ISessionRepository
    {
        public void Add(Session entity)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(Session entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Session entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Session entity)
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

        public Session Get(Session entity)
        {
            throw new NotImplementedException();
        }

        public ICollection<Session> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Session>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Session> GetAsync(Session entity)
        {
            throw new NotImplementedException();
        }

        public Session GetById(long id)
        {
            throw new NotImplementedException();
        }

        public Task<Session> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public void Update(Session entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Session entity)
        {
            throw new NotImplementedException();
        }
    }
}
