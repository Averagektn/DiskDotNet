using Disk.Repository.Common.Interface;
using Disk.Repository.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Disk.Repository.Common.Implementation
{
    public class CrudRepository<T>(DbContext context) : ICrudRepository<T> where T : class
    {
        protected readonly DbSet<T> table = context.Set<T>();

        public void Add(T entity)
        {
            _ = table.Add(entity);
            try
            {
                _ = context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                _ = context.Remove(entity);
                throw;
            }
        }

        public async Task AddAsync(T entity)
        {
            _ = await table.AddAsync(entity);
            try
            {
                _ = await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                _ = context.Remove(entity);
                throw;
            }
        }

        public void Delete(T entity)
        {
            _ = table.Remove(entity);
            _ = context.SaveChanges();
        }

        public ICollection<T> GetAll() => table.ToList();

        public async Task<ICollection<T>> GetAllAsync() => await table.ToListAsync();

        public T GetById(long id) =>
            table.Find(id) ?? throw new EntityNotFoundException($"No such {typeof(T)} with id {id}");

        public async Task<T> GetByIdAsync(long id)
            => await table.FindAsync(id) ?? throw new EntityNotFoundException($"No such {typeof(T)} with id {id}");

        public void Update(T entity)
        {
            _ = table.Update(entity);
            _ = context.SaveChanges();
        }
    }
}
