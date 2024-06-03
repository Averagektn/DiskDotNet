using Disk.Db.Context;
using Disk.Entities;
using Disk.Repository.Common;
using Disk.Repository.Interface;

namespace Disk.Repository.Implementation
{
    public class DoctorRepository(DiskContext diskContext) : IDoctorRepository, IAuthRepository<Doctor>
    {
        public Doctor PerformAuthorization(Doctor entity)
        {
            throw new NotImplementedException();
        }

        public Doctor PerformRegistration(Doctor entity)
        {
            throw new NotImplementedException(); 
        }

        public Task<Doctor> PerformAuthorizationAsync(Doctor entity)
        {
            throw new NotImplementedException();
        }

        public Task<Doctor> PerformRegistrationAsync(Doctor entity)
        {
            throw new NotImplementedException();
        }

        public void Add(Doctor entity)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(Doctor entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Doctor entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Doctor entity)
        {
            throw new NotImplementedException();
        }

        public void DeleteById(long id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Doctor Get(Doctor entity)
        {
            throw new NotImplementedException();
        }

        public ICollection<Doctor> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Doctor>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Doctor> GetAsync(Doctor entity)
        {
            throw new NotImplementedException();
        }

        public Doctor GetById(long id)
        {
            throw new NotImplementedException();
        }

        public Task<Doctor> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public void Update(Doctor entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Doctor entity)
        {
            throw new NotImplementedException();
        }
    }
}
