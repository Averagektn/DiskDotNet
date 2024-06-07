using Disk.Db.Context;
using Disk.Entities;
using Disk.Repository.Common;
using Disk.Repository.Common.Implementation;
using Disk.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Disk.Repository.Implementation
{
    public class DoctorRepository(DiskContext diskContext) : CrudRepository<Doctor>(diskContext), IDoctorRepository,
        IAuthRepository<Doctor>
    {
        public bool PerformAuthorization(Doctor entity)
        {
            return table.Any(d => entity.Name == d.Name && entity.Surname == d.Surname && entity.Patronymic == d.Patronymic
                && entity.Password == d.Password);
        }

        public async Task<bool> PerformAuthorizationAsync(Doctor entity)
        {
            return await table.AnyAsync(d => entity.Name == d.Name && entity.Surname == d.Surname &&
                entity.Patronymic == d.Patronymic && entity.Password == d.Password);
        }

        public Doctor PerformRegistration(Doctor entity)
        {
            Add(entity);

            return entity;
        }

        public async Task<Doctor> PerformRegistrationAsync(Doctor entity)
        {
            await AddAsync(entity);

            return entity;
        }
    }
}
