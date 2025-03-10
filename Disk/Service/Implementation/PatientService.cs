using Disk.Db.Context;
using Disk.Entities;
using Disk.Properties.Langs.ServiceException;
using Disk.Service.Exceptions;
using Disk.Service.Interface;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Disk.Service.Implementation;

public class PatientService(DiskContext database) : IPatientService
{
    public void Add(Patient patient)
    {
        if (Validate(patient))
        {
            _ = database.Add(patient);
        }
    }

    public async Task AddAsync(Patient patient)
    {
        if (Validate(patient))
        {
            var isPossibleDuplicate = await database.Patients.AnyAsync(p =>
                p.Name == patient.Name &&
                p.Surname == patient.Surname &&
                p.Patronymic == patient.Patronymic &&
                p.DateOfBirth == patient.DateOfBirth);
            if (isPossibleDuplicate)
            {
                throw new PossibleDuplicateEntityException("Same phone number");
            }

            _ = await database.AddAsync(patient);
            _ = await database.SaveChangesAsync();
        }
    }

    public void Update(Patient patient)
    {
        if (Validate(patient))
        {
            _ = database.Update(patient);
            _ = database.SaveChanges();
        }
    }

    public async Task UpdateAsync(Patient patient)
    {
        if (Validate(patient))
        {
            _ = database.Update(patient);
            _ = await database.SaveChangesAsync();
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
}
