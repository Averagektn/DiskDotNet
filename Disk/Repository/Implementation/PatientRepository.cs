using Disk.Db.Context;
using Disk.Entities;
using Disk.Repository.Interface;

namespace Disk.Repository.Implementation
{
    public class PatientRepository(DiskContext diskContext) : IPatientRepository
    {
        public void Add(Patient entity)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(Patient entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Patient entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Patient entity)
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

        public Patient Get(Patient entity)
        {
            throw new NotImplementedException();
        }

        public ICollection<Patient> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Patient>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Patient> GetAsync(Patient entity)
        {
            throw new NotImplementedException();
        }

        public Patient GetById(long id)
        {
            throw new NotImplementedException();
        }

        public Task<Patient> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public void Update(Patient entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Patient entity)
        {
            throw new NotImplementedException();
        }
    }
}
