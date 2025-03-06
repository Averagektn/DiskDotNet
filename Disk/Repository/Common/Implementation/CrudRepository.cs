using Disk.Properties.Langs.RepositoryException;
using Disk.Repository.Common.Interface;
using Disk.Repository.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Disk.Repository.Common.Implementation;

public class CrudRepository<T>(DbContext context) : ICrudRepository<T> where T : class
{
    protected readonly DbSet<T> Table = context.Set<T>();

    public void Add(T entity)
    {
        _ = Table.Add(entity);
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
        _ = await Table.AddAsync(entity);
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
        _ = Table.Remove(entity);
        _ = context.SaveChanges();
    }

    public ICollection<T> GetAll()
    {
        return [.. Table];
    }

    public async Task<ICollection<T>> GetAllAsync() => await Table.ToListAsync();

    public T GetById(long id)
    {
        return Table.Find(id)
            ?? throw new EntityNotFoundException($"No such {typeof(T)} with id {id}", RepositoryExceptionLocalization.NoSuchItem);
    }

    public async Task<T> GetByIdAsync(long id)
    {
        return await Table.FindAsync(id)
            ?? throw new EntityNotFoundException($"No such {typeof(T)} with id {id}", RepositoryExceptionLocalization.NoSuchItem);
    }

    public void Update(T entity)
    {
        _ = Table.Update(entity);
        _ = context.SaveChanges();
    }
}
