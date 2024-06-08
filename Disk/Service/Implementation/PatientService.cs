using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Service.Interface;

namespace Disk.Service.Implementation
{
    public class PatientService(IPatientRepository patientRepository) : IPatientService
    {
        public void AddPatient(Patient patient)
        {
            if (Validate(patient))
            {
                patientRepository.Add(patient);
            }
        }

        public async Task AddPatientAsync(Patient patient)
        {
            if (Validate(patient))
            {
                await patientRepository.AddAsync(patient);
            }
        }

        private bool Validate(Patient patient)
        {
            // validate phone number
            // validate date
            return true;
        }
    }
}
