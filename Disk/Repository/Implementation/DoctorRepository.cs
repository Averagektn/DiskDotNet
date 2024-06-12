using Disk.Db.Context;
using Disk.Entities;
using Disk.Properties.Langs.Authentication;
using Disk.Repository.Common.Implementation;
using Disk.Repository.Common.Interface;
using Disk.Repository.Exceptions;
using Disk.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Disk.Repository.Implementation
{
    public class DoctorRepository(DiskContext diskContext) : CrudRepository<Doctor>(diskContext), IDoctorRepository,
        IAuthRepository<Doctor>
    {
        public bool PerformAuthorization(Doctor entity)
        {
            var doctors = table
                .Where(d => entity.Name == d.Name && entity.Surname == d.Surname && entity.Patronymic == d.Patronymic)
                .ToList();
            var doctor = doctors.FirstOrDefault();

            if (doctor is not null)
            {
                entity = doctor;
                return true;
            }
            return false;
        }

        public async Task<bool> PerformAuthorizationAsync(Doctor entity)
        {
            var doctors = await table
                .Where(d => entity.Name == d.Name && entity.Surname == d.Surname && entity.Patronymic == d.Patronymic)
                .ToListAsync();
            var doctor = doctors.FirstOrDefault();

            if (doctor is not null)
            {
                entity.Id = doctor.Id;
                return true;
            }
            return false;
        }

        public Doctor PerformRegistration(Doctor entity)
        {
            var doctors = table
                .Where(d => entity.Name == d.Name && entity.Surname == d.Surname && entity.Patronymic == d.Patronymic)
                .ToList();

            if (doctors.Count != 0)
            {
                throw new DuplicateEntityException(AuthenticationLocalization.Duplication, $"Duplicated entity {entity}");
            }

            Add(entity);
            return entity;
        }

        public async Task<Doctor> PerformRegistrationAsync(Doctor entity)
        {
            var doctors = await table
                .Where(d => entity.Name == d.Name && entity.Surname == d.Surname && entity.Patronymic == d.Patronymic)
                .ToListAsync();

            if (doctors.Count != 0)
            {
                throw new DuplicateEntityException(AuthenticationLocalization.Duplication, $"Duplicated entity {entity}");
            }

            await AddAsync(entity);
            return entity;
        }
    }
}
