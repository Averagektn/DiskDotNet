using Microsoft.EntityFrameworkCore;
using Disk.Repository.Exceptions;

namespace Disk.Repository.Common.Implementation
{
    public class CrudRepository<T>(DbContext context) : ICrudRepository<T> where T : class
    {
        protected readonly DbSet<T> table = context.Set<T>();

        public void Add(T entity)
        {
            table.Add(entity);
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                context.Remove(entity);
                throw;
            }
        }

        public async Task AddAsync(T entity)
        {
            await table.AddAsync(entity);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                context.Remove(entity);
                throw;
            }
        }

        public void Delete(T entity)
        {
            table.Remove(entity);
            context.SaveChanges();
        }

        public ICollection<T> GetAll()
        {
            return table.ToList();
        }

        public async Task<ICollection<T>> GetAllAsync()
        {
            return await table.ToListAsync();
        }

        public T GetById(long id)
        {
            return table.Find(id) ?? throw new EntityNotFoundException($"No such {typeof(T)} with id {id}");
        }

        public async Task<T> GetByIdAsync(long id)
        {
            return await table.FindAsync(id) ?? throw new EntityNotFoundException($"No such {typeof(T)} with id {id}");
        }

        public void Update(T entity)
        {
            table.Update(entity);
            context.SaveChanges();
        }
    }
}
