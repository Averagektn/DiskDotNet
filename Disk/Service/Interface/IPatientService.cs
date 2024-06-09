using Disk.Entities;

namespace Disk.Service.Interface
{
    public interface IPatientService
    {
        void AddPatient(Patient patient);
        Task AddPatientAsync(Patient patient);
    }
}
