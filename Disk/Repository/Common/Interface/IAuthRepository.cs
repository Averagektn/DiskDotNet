﻿namespace Disk.Repository.Common
{
    public interface IAuthRepository<Entity> where Entity : class
    {
        bool PerformAuthorization(Entity entity);
        Entity PerformRegistration(Entity entity);

        Task<bool> PerformAuthorizationAsync(Entity entity);
        Task<Entity> PerformRegistrationAsync(Entity entity);
    }
}
