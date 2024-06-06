namespace Disk.Repository.Common
{
    public interface IAuthRepository<Entity> where Entity : class
    {
        bool PerformAuthorization(Entity entity);
        bool PerformRegistration(Entity entity);

        Task<bool> PerformAuthorizationAsync(Entity entity);
        Task<bool> PerformRegistrationAsync(Entity entity);
    }
}
