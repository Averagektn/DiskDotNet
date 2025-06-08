using System.Globalization;

using Disk.Db.Context;
using Disk.Entities;
using Disk.Properties.Langs.ServiceException;
using Disk.Services.Exceptions;
using Disk.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Disk.Services.Implementations;

public class PatientService(DiskContext database) : IPatientService
{
    public void CheckDuplicateAndAdd(Patient patient)
    {
        if (!Validate(patient))
        {
            return;
        }

        bool isExactDuplicate = database.Patients.Any(p =>
            p.Name == patient.Name &&
            p.Surname == patient.Surname &&
            p.Patronymic == patient.Patronymic &&
            p.DateOfBirth == patient.DateOfBirth &&
            p.PhoneMobile == patient.PhoneMobile);

        if (isExactDuplicate)
        {
            throw new DuplicateEntityException("Duplicate patient found");
        }

        bool isPossibleDuplicate = database.Patients.Any(p =>
            p.Name == patient.Name &&
            p.Surname == patient.Surname &&
            p.Patronymic == patient.Patronymic &&
            p.DateOfBirth == patient.DateOfBirth);

        if (isPossibleDuplicate)
        {
            throw new PossibleDuplicateEntityException("Possible duplication of patient");
        }

        _ = database.Add(patient);
        _ = database.SaveChanges();
    }

    public async Task CheckDuplicateAndAddAsync(Patient patient)
    {
        if (!Validate(patient))
        {
            return;
        }

        bool isExactDuplicate = await database.Patients.AnyAsync(p =>
            p.Name == patient.Name &&
            p.Surname == patient.Surname &&
            p.Patronymic == patient.Patronymic &&
            p.DateOfBirth == patient.DateOfBirth &&
            p.PhoneMobile == patient.PhoneMobile);

        if (isExactDuplicate)
        {
            throw new DuplicateEntityException("Duplicate patient found");
        }

        bool isPossibleDuplicate = await database.Patients.AnyAsync(p =>
            p.Name == patient.Name &&
            p.Surname == patient.Surname &&
            p.Patronymic == patient.Patronymic &&
            p.DateOfBirth == patient.DateOfBirth);

        if (isPossibleDuplicate)
        {
            throw new PossibleDuplicateEntityException("Possible duplication of patient");
        }

        _ = await database.AddAsync(patient);
        _ = await database.SaveChangesAsync();
    }

    public void CheckDuplicateAndUpdate(Patient patient)
    {
        if (!Validate(patient))
        {
            return;
        }

        bool isExactDuplicate = database.Patients.Any(p =>
            p.Name == patient.Name &&
            p.Surname == patient.Surname &&
            p.Patronymic == patient.Patronymic &&
            p.DateOfBirth == patient.DateOfBirth &&
            p.PhoneMobile == patient.PhoneMobile &&
            p.Id != patient.Id);

        if (isExactDuplicate)
        {
            throw new DuplicateEntityException("Duplicate patient found");
        }

        bool isPossibleDuplicate = database.Patients.Any(p =>
            p.Name == patient.Name &&
            p.Surname == patient.Surname &&
            p.Patronymic == patient.Patronymic &&
            p.DateOfBirth == patient.DateOfBirth &&
            p.Id != patient.Id);

        if (isPossibleDuplicate)
        {
            throw new PossibleDuplicateEntityException("Possible duplication of patient");
        }

        _ = database.Update(patient);
        _ = database.SaveChanges();
    }

    public async Task CheckDuplicateAndUpdateAsync(Patient patient)
    {
        if (!Validate(patient))
        {
            return;
        }

        bool isExactDuplicate = await database.Patients.AnyAsync(p =>
            p.Name == patient.Name &&
            p.Surname == patient.Surname &&
            p.Patronymic == patient.Patronymic &&
            p.DateOfBirth == patient.DateOfBirth &&
            p.PhoneMobile == patient.PhoneMobile &&
            p.Id != patient.Id);

        if (isExactDuplicate)
        {
            throw new DuplicateEntityException("Duplicate patient found");
        }

        bool isPossibleDuplicate = await database.Patients.AnyAsync(p =>
            p.Name == patient.Name &&
            p.Surname == patient.Surname &&
            p.Patronymic == patient.Patronymic &&
            p.DateOfBirth == patient.DateOfBirth &&
            p.Id != patient.Id);

        if (isPossibleDuplicate)
        {
            throw new PossibleDuplicateEntityException("Possible duplication of patient");
        }

        _ = database.Update(patient);
        _ = await database.SaveChangesAsync();
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
        return date.Date >= DateTime.Now.Date
            ? throw new InvalidDateException("Patient add date exception", ServiceExceptionLocalization.DateFormatException)
            : true;
    }
}
