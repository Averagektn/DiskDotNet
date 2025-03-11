using Disk.Entities;

namespace Disk.Service.Interface;

public interface IPatientService
{
    void CheckDuplicateAndAdd(Patient patient);
    Task CheckDuplicateAndAddAsync(Patient patient);

    void CheckDuplicateAndUpdate(Patient patient);
    Task CheckDuplicateAndUpdateAsync(Patient patient);
}
