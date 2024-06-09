using Disk.Entities;
using Disk.Properties.Langs.Authentication;
using Disk.Repository.Exceptions;
using Disk.Repository.Exceptions.Common;
using Disk.Repository.Interface;
using Disk.Service.Exceptions;
using Disk.Service.Exceptions.Common;
using Disk.Service.Interface;
using Serilog;
using System.Security.Cryptography;
using System.Text;

namespace Disk.Service.Implementation
{
    public class AuthenticationService(IDoctorRepository doctorRepository) : IAuthenticationService
    {
        public async Task<bool> PerformAuthorizationAsync(Doctor doctor)
        {
            bool result;
            _ = ValidateDoctor(doctor);

            try
            {
                var hash = SHA512.HashData(Encoding.UTF8.GetBytes(doctor.Password));
                doctor.Password = Encoding.Default.GetString(hash);
                result = await doctorRepository.PerformAuthorizationAsync(doctor);
            }
            catch (RepositoryException ex)
            {
                Log.Fatal(ex.Message);
                throw new ServiceException(AuthenticationLocalization.DbError, ex.Message);
            }

            return result;
        }

        public async Task<Doctor> PerformRegistrationAsync(Doctor doctor)
        {
            Doctor result;
            _ = ValidateDoctor(doctor);

            try
            {
                var hash = SHA512.HashData(Encoding.UTF8.GetBytes(doctor.Password));
                doctor.Password = Encoding.Default.GetString(hash);
                result = await doctorRepository.PerformRegistrationAsync(doctor);
            }
            catch (DuplicateEntityException ex)
            {
                Log.Error(ex.Message);
                throw;
            }
            catch (RepositoryException ex)
            {
                Log.Fatal(ex.Message);
                throw new ServiceException(AuthenticationLocalization.DbError, ex.Message);
            }

            return result;
        }

        private static bool ValidateDoctor(Doctor doctor)
        {
            if (doctor.Surname.Trim().Length == 0)
            {
                Log.Error("Empty surname");
                throw new InvalidSurnameException(AuthenticationLocalization.EmptySurname, "Surname is not entered");
            }
            if (doctor.Name.Trim().Length == 0)
            {
                Log.Error("Empty name");
                throw new InvalidNameException(AuthenticationLocalization.EmptyName, "Name is not entered");
            }
            if (doctor.Password.Trim().Length < 8)
            {
                Log.Error("Invalid password");
                throw new InvalidPasswordException(AuthenticationLocalization.ShortPassword, "Invalid passsword length");
            }

            return true;
        }
    }
}
