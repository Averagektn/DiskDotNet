﻿using Disk.Db.Context;
using Disk.Entities;
using Disk.Properties.Langs.ServiceException;
using Disk.Service.Exceptions;
using Disk.Service.Interface;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Disk.Service.Implementation;

public class PatientService(DiskContext database) : IPatientService
{
    public void CheckDuplicateAndAdd(Patient patient)
    {
        if (!Validate(patient))
        {
            return;
        }

        var isExactDuplicate = database.Patients.Any(p =>
            p.Name == patient.Name &&
            p.Surname == patient.Surname &&
            p.Patronymic == patient.Patronymic &&
            p.DateOfBirth == patient.DateOfBirth &&
            p.PhoneMobile == patient.PhoneMobile);

        if (isExactDuplicate)
        {
            throw new DuplicateEntityException("Duplicate patient found");
        }

        var isPossibleDuplicate = database.Patients.Any(p =>
            p.Name == patient.Name &&
            p.Surname == patient.Surname &&
            p.Patronymic == patient.Patronymic &&
            p.DateOfBirth == patient.DateOfBirth);

        if (isPossibleDuplicate)
        {
            throw new PossibleDuplicateEntityException("Possible duplication of patient");
        }

        database.Add(patient);
        database.SaveChanges();
    }

    public async Task CheckDuplicateAndAddAsync(Patient patient)
    {
        if (!Validate(patient))
        {
            return;
        }

        var isExactDuplicate = await database.Patients.AnyAsync(p =>
            p.Name == patient.Name &&
            p.Surname == patient.Surname &&
            p.Patronymic == patient.Patronymic &&
            p.DateOfBirth == patient.DateOfBirth &&
            p.PhoneMobile == patient.PhoneMobile);

        if (isExactDuplicate)
        {
            throw new DuplicateEntityException("Duplicate patient found");
        }

        var isPossibleDuplicate = await database.Patients.AnyAsync(p =>
            p.Name == patient.Name &&
            p.Surname == patient.Surname &&
            p.Patronymic == patient.Patronymic &&
            p.DateOfBirth == patient.DateOfBirth);

        if (isPossibleDuplicate)
        {
            throw new PossibleDuplicateEntityException("Possible duplication of patient");
        }

        await database.AddAsync(patient);
        await database.SaveChangesAsync();
    }

    public void CheckDuplicateAndUpdate(Patient patient)
    {
        if (!Validate(patient))
        {
            return;
        }

        var isExactDuplicate = database.Patients.Any(p =>
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

        var isPossibleDuplicate = database.Patients.Any(p =>
            p.Name == patient.Name &&
            p.Surname == patient.Surname &&
            p.Patronymic == patient.Patronymic &&
            p.DateOfBirth == patient.DateOfBirth &&
            p.Id != patient.Id);

        if (isPossibleDuplicate)
        {
            throw new PossibleDuplicateEntityException("Possible duplication of patient");
        }

        database.Update(patient);
        database.SaveChanges();
    }

    public async Task CheckDuplicateAndUpdateAsync(Patient patient)
    {
        if (!Validate(patient))
        {
            return;
        }

        var isExactDuplicate = await database.Patients.AnyAsync(p =>
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

        var isPossibleDuplicate = await database.Patients.AnyAsync(p =>
            p.Name == patient.Name &&
            p.Surname == patient.Surname &&
            p.Patronymic == patient.Patronymic &&
            p.DateOfBirth == patient.DateOfBirth &&
            p.Id != patient.Id); 

        if (isPossibleDuplicate)
        {
            throw new PossibleDuplicateEntityException("Possible duplication of patient");
        }

        database.Update(patient);
        await database.SaveChangesAsync();
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
