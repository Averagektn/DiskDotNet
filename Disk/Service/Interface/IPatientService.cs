using Disk.Entities;

namespace Disk.Service.Interface;

public interface IPatientService
{
    void Add(Patient patient);
    Task AddAsync(Patient patient);

    void Update(Patient patient);
    Task UpdateAsync(Patient patient);
}
