using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Service.Exceptions;
using Disk.Service.Interface;
using System.Globalization;

namespace Disk.Service.Implementation
{
    public class PatientService(IPatientRepository patientRepository) : IPatientService
    {
        public void Add(Patient patient)
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
            if (patient.Surname == string.Empty)
            {
                throw new InvalidSurnameException("Surname is empty");
            }
            if (patient.Name == string.Empty)
            {
                throw new InvalidNameException("Name is empty");
            }
            if (patient.DateOfBirth == string.Empty)
            {
                throw new InvalidDateException("Empty date");
            }
            if (patient.PhoneMobile == string.Empty)
            {
                throw new InvalidPhoneNumberException("Mobile phone is empty");
            }
            if (patient.PhoneHome == string.Empty)
            {
                throw new InvalidHomePhoneException("Home phone is empty");
            }

            const int mobilePhoneLength = 13;
            try
            {
                patient.DateOfBirth = DateTime.Parse(patient.DateOfBirth, CultureInfo.CurrentCulture).ToShortDateString();
            }
            catch
            {
                patient.DateOfBirth = DateTime.Parse(patient.DateOfBirth, CultureInfo.InvariantCulture).ToShortDateString();
            }
            var date = DateTime.Parse(patient.DateOfBirth);

            if (date.Date >= DateTime.Now.Date)
            {
                throw new InvalidDateException("Patient add date exception");
            }
            return !patient.PhoneMobile.StartsWith("+375") || patient.PhoneMobile.Length < mobilePhoneLength
                ? throw new InvalidPhoneNumberException("Patient mobile phone exception")
                : true;
        }

        public void Update(Patient patient)
        {
            if (Validate(patient))
            {
                patientRepository.Update(patient);
            }
        }
    }
}
