using Disk.Db.Context;
using Disk.Entities;
using Disk.Repository.Common;
using Disk.Repository.Common.Implementation;
using Disk.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Disk.Repository.Exceptions;
using Disk.Properties.Langs.Authentication;

namespace Disk.Repository.Implementation
{
    public class DoctorRepository(DiskContext diskContext) : CrudRepository<Doctor>(diskContext), IDoctorRepository,
        IAuthRepository<Doctor>
    {
        public bool PerformAuthorization(Doctor entity)
        {
            var doctors = table.Where(d => entity.Name == d.Name && entity.Surname == d.Surname && entity.Patronymic == d.Patronymic
                && entity.Password == d.Password).ToList();
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
            var doctors = await table.Where(d => entity.Name == d.Name && entity.Surname == d.Surname && 
                entity.Patronymic == d.Patronymic && entity.Password == d.Password).ToListAsync();
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
            var doctors = table.Where(d => entity.Name == d.Name && entity.Surname == d.Surname &&
                entity.Patronymic == d.Patronymic && entity.Password == d.Password).ToList();
            if (doctors.Count != 0)
            {
                throw new DuplicateEntityException(AuthenticationLocalization.Duplication, $"Duplicated entity {entity}");
            }

            Add(entity);

            return entity;
        }

        public async Task<Doctor> PerformRegistrationAsync(Doctor entity)
        {
            var doctors = await table.Where(d => entity.Name == d.Name && entity.Surname == d.Surname &&
                entity.Patronymic == d.Patronymic && entity.Password == d.Password).ToListAsync();
            if (doctors.Count != 0)
            {
                throw new DuplicateEntityException(AuthenticationLocalization.Duplication, $"Duplicated entity {entity}");
            }

            await AddAsync(entity);

            return entity;
        }
    }
}
