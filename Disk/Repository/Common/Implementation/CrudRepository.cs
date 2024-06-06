using Microsoft.EntityFrameworkCore;
using Disk.Repository.Exceptions;

namespace Disk.Repository.Common.Implementation
{
    public class CrudRepository<T>(DbContext context) : ICrudRepository<T> where T : class
    {
        protected readonly DbSet<T> table = context.Set<T>();

        public void Add(T entity)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public ICollection<T> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<T>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public T GetById(long id)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
