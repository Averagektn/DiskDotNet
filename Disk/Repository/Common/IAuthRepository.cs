namespace Disk.Repository.Common
{
    public interface IAuthRepository<Entity>
    {
        Entity PerformAuthorization(Entity entity);
        Entity PerformRegistration(Entity entity);

        Task<Entity> PerformAuthorizationAsync(Entity entity);
        Task<Entity> PerformRegistrationAsync(Entity entity);
    }
}
