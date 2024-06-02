namespace Disk.Repository.Common
{
    public interface ICrudIdRepository<Entity> : ICrudRepository<Entity>
    {
        Entity GetById(long id);
        void DeleteById(long id);

        Task<Entity> GetByIdAsync(long id);
        Task DeleteByIdAsync(long id);
    }
}
