using Disk.Db.Context;
using Disk.Entities;
using Disk.Repository.Common.Implementation;
using Disk.Repository.Exceptions;
using Disk.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Disk.Repository.Implementation
{
    public class PatientRepository(DiskContext diskContext) : CrudRepository<Patient>(diskContext), IPatientRepository
    {
        public new void Add(Patient entity)
        {
            var patientExists = table.Any(p => p.Name == entity.Name && p.Surname == entity.Surname &&
                p.Patronymic == entity.Patronymic && p.DateOfBirth == entity.DateOfBirth);
            if (patientExists)
            {
                throw new DuplicateEntityException("Patient duplication");
            }

            base.Add(entity);
        }

        public new async Task AddAsync(Patient entity)
        {
            var patientExists = await table.AnyAsync(p => p.Name == entity.Name && p.Surname == entity.Surname &&
                p.Patronymic == entity.Patronymic && p.DateOfBirth == entity.DateOfBirth);
            if (patientExists)
            {
                throw new DuplicateEntityException("Patient duplication");
            }

            await base.AddAsync(entity);
        }
    }
}
