namespace Disk.Repository.Common
{
    public interface ICrudRepository<Entity> where Entity : class
    {
        void Add(Entity entity);
        ICollection<Entity> GetAll();
        void Update(Entity entity);
        Entity GetById(long id);
        void Delete(Entity entity);

        Task AddAsync(Entity entity);
        Task<ICollection<Entity>> GetAllAsync();
        Task<Entity> GetByIdAsync(long id);
    }
}
