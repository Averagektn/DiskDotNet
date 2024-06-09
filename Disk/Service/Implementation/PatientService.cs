using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Service.Interface;
using Disk.Service.Exceptions;

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

        private static bool Validate(Patient patient)
        {
            if (patient.DateOfBirth == string.Empty)
            {
                throw new InvalidDateException("");
            }
            if (patient.PhoneMobile == string.Empty)
            {
                throw new InvalidPhoneNumberException("Phone is empty");
            }
            if (patient.PhoneHome == string.Empty)
            {
                throw new InvalidHomePhoneException("Home phone is empty");
            }
            if (patient.Surname == string.Empty)
            {
                throw new InvalidSurnameException("");
            }
            if (patient.Name == string.Empty)
            {
                throw new InvalidNameException("");
            }

            const int mobilePhoneLength = 13;
            patient.DateOfBirth = DateTime.Parse(patient.DateOfBirth).ToShortDateString();
            var date = DateTime.Parse(patient.DateOfBirth);
  
            if (date.Date >= DateTime.Now.Date)
            {
                throw new InvalidDateException("Invalid date", "Patient add date exception");
            }
            if (!patient.PhoneMobile.StartsWith("+375") || patient.PhoneMobile.Length < mobilePhoneLength)
            {
                throw new InvalidPhoneNumberException("Invalid mobile phone", "Patient mobile phone exception");
            }
            return true;
        }
    }
}
