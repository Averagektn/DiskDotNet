using Disk.Entities;

namespace Disk.Service.Interface
{
    public interface IAuthenticationService
    {
        Task<bool> PerformAuthorizationAsync(Doctor doctor);
        Task<Doctor> PerformRegistrationAsync(Doctor doctor);
    }
}
