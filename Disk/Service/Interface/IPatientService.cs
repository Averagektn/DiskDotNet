using Disk.Entities;

namespace Disk.Service.Interface;

public interface IPatientService
{
    void Add(Patient patient);
    Task AddPatientAsync(Patient patient);

    void Update(Patient patient);
}
