using Disk.Db.Context;
using Disk.Entities;
using Disk.Repository.Interface;

namespace Disk.Repository.Implementation
{
    public class AppointmentRepository(DiskContext diskContext) : IAppointmentRepository
    {
        public void Add(Appointment entity)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(Appointment entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Appointment entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Appointment entity)
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

        public Appointment Get(Appointment entity)
        {
            throw new NotImplementedException();
        }

        public ICollection<Appointment> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Appointment>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Appointment> GetAsync(Appointment entity)
        {
            throw new NotImplementedException();
        }

        public Appointment GetById(long id)
        {
            throw new NotImplementedException();
        }

        public Task<Appointment> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public void Update(Appointment entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Appointment entity)
        {
            throw new NotImplementedException();
        }
    }
}
