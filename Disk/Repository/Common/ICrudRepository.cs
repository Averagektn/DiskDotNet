namespace Disk.Repository.Common
{
    public interface ICrudRepository<Entity>
    {
        void Add(Entity entity);
        Entity Get(Entity entity);
        ICollection<Entity> GetAll();
        void Update(Entity entity);
        void Delete(Entity entity);

        Task AddAsync(Entity entity);
        Task<Entity> GetAsync(Entity entity);
        Task<ICollection<Entity>> GetAllAsync();
        Task UpdateAsync(Entity entity);
        Task DeleteAsync(Entity entity);
    }
}
