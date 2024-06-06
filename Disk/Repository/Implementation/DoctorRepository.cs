using Disk.Db.Context;
using Disk.Entities;
using Disk.Repository.Common;
using Disk.Repository.Common.Implementation;
using Disk.Repository.Interface;

namespace Disk.Repository.Implementation
{
    public class DoctorRepository(DiskContext diskContext) : CrudRepository<Doctor>(diskContext), IDoctorRepository, 
        IAuthRepository<Doctor>
    {
        public bool PerformAuthorization(Doctor entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PerformAuthorizationAsync(Doctor entity)
        {
            throw new NotImplementedException();
        }

        public bool PerformRegistration(Doctor entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PerformRegistrationAsync(Doctor entity)
        {
            throw new NotImplementedException();
        }
    }
}
