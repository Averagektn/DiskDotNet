using Disk.Entities;
using Disk.Repository.Common.Interface;

namespace Disk.Repository.Interface
{
    public interface IDoctorRepository : ICrudRepository<Doctor>, IAuthRepository<Doctor> { }
}
