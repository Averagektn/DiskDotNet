using Disk.Entities;
using Disk.Properties.Langs.ServiceException;
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
                throw new InvalidSurnameException("Surname is empty", ServiceExceptionLocalization.EmptySurname);
            }
            if (patient.Name == string.Empty)
            {
                throw new InvalidNameException("Name is empty", ServiceExceptionLocalization.EmptyName);
            }
            if (patient.DateOfBirth == string.Empty)
            {
                throw new InvalidDateException("Empty date", ServiceExceptionLocalization.EmptyDate);
            }
            if (patient.PhoneMobile == string.Empty)
            {
                throw new InvalidPhoneNumberException("Mobile phone is empty", ServiceExceptionLocalization.EmptyMobilePhone);
            }

            var date = DateTime.ParseExact(patient.DateOfBirth, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            if (date.Date >= DateTime.Now.Date)
            {
                throw new InvalidDateException("Patient add date exception", ServiceExceptionLocalization.DateFormatException);
            }

            return true;
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
